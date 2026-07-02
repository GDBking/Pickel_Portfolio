using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileProb
{
    public TileData tileData;
    public float prob;
}

[RequireComponent(typeof(Tilemap))]
public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    [Header("Tilemap")]
    [HideInInspector] public Tilemap tilemap;

    [Header("Tile Settings")]
    public TileProb[] tileDatas;
    int oreTileProb;
    [HideInInspector] public int orePieceMin, orePieceMax;
    float[] sumTileProbs;
    [SerializeField] GameObject orePrefab;
    [SerializeField] Transform orePool;
    [HideInInspector] public Dictionary<Vector3Int, GameObject> posInOre = new();
    [SerializeField] TileBase entranceTile;

    [Header("Item Settings")]
    [SerializeField] GameObject[] bombObjs;
    [SerializeField] GameObject[] potionObjs;
    [SerializeField] Transform itemPool;
    readonly List<Vector3Int> emptyTileList = new();
    [HideInInspector] public Dictionary<Vector3Int, GameObject> posInItems = new();

    [Header("NPC List")]
    [SerializeField] GameObject[] npcList;

    [Header("Size")]
    int width;
    int height;

    [Header("Mine UI")]
    public MineBag[] bags;

    [Header("Wall")]
    [SerializeField] Transform leftWall;
    [SerializeField] Transform rightWall;

    [Header("Instance")]
    [SerializeField] TotalPanel totalPanel;

    bool isDungeonEntrance;

    private void Awake()
    {
        Instance = this;
        TotalPanel.Instance = totalPanel;

        tilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        OrePiece.lastBagSoundTime = 0f;

        var elderData = SaveManager.saveData.village.elder;
        sumTileProbs = new float[(int)elderData.oreIdx + 1];
        tileDatas[0].tileData.maxHP = elderData.baseTileHP;
        tileDatas[^1].tileData.maxHP = elderData.baseTileHP;

        isDungeonEntrance = SaveManager.saveData.npc.unlockIdx == 13;

        SetTileProb();
        AlignTopCenter();
        Generate();
        SetWallPos();
    }

    void SetTileProb()
    {
        // 테두리 크기 추가
        var elderData = SaveManager.saveData.village.elder;
        width = elderData.x + 2;
        height = elderData.y + 2;

        sumTileProbs[0] = tileDatas[2].prob;
        for (int i = 1; i < sumTileProbs.Length; i++)
        {
            sumTileProbs[i] = sumTileProbs[i - 1] + tileDatas[i + 2].prob;
        }

        var lunaData = SaveManager.saveData.village.luna;
        oreTileProb = lunaData.oreProb;
        orePieceMin = lunaData.orePieceMin;
        orePieceMax = lunaData.orePieceMax;
        tileDatas[1].prob = lunaData.obstacleProb;
    }

    void Generate()
    {
        BoundsInt bounds = new(0, 0, 0, width, height, 1);
        TileBase[] tileArray = new TileBase[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;
                tileArray[index] = GetTile(new(x, y));
            }
        }
        GenerateItems(SaveManager.saveData.village.boom.bombCnt, bombObjs);
        GenerateItems(SaveManager.saveData.village.pora.potionCnt, potionObjs);

        var npcData = SaveManager.saveData.npc;
        if (npcData.isUnlock && npcData.unlockIdx < npcList.Length && SaveManager.saveData.isInit)
        {
            if ((npcData.unlockIdx <= 1 || npcData.unlockIdx == 12) || Random.Range(0, 3) == 0)
            {
                if (npcList[npcData.unlockIdx].GetComponent<NPC>().isOnTheGround)
                {
                    Vector3 worldPos = tilemap.GetCellCenterWorld(new(width / 2 - 2, height));
                    Instantiate(npcList[npcData.unlockIdx], worldPos, Quaternion.identity);

                    int index = (width / 2 - 2) + (height - 1) * width;
                    for (int i = index - 1; i <= index + 1; i++)
                        tileArray[i] = tileDatas[^2].tileData.baseTile;
                }
                else
                {
                    int x = Random.Range(1, width - 3);
                    int y = Random.Range(1, height / 2);

                    for (int y2 = y; y2 < y + 3; y2++)
                    {
                        for (int x2 = x; x2 < x + 3; x2++)
                        {
                            if (posInOre.TryGetValue(new(x2, y2), out GameObject ore))
                            {
                                Destroy(ore);
                                posInOre.Remove(new(x2, y2));
                            }
                            else if (posInItems.TryGetValue(new(x2, y2), out GameObject item))
                            {
                                Destroy(item);
                                posInItems.Remove(new(x2, y2));
                            }

                            int index = x2 + y2 * width;
                            if (y2 == y || y2 == y + 2)
                                tileArray[index] = tileDatas[1].tileData.baseTile;
                            else
                                tileArray[index] = null;
                        }
                    }

                    Vector3 worldPos = tilemap.GetCellCenterWorld(new(x + 1, y + 1));
                    Instantiate(npcList[npcData.unlockIdx], worldPos, Quaternion.identity);
                }
            }
        }

        tilemap.SetTilesBlock(bounds, tileArray);
    }

    TileBase GetTile(Vector3Int cellPos)
    {
        int x = cellPos.x;
        int y = cellPos.y;

        // 1. 좌우 + 아래 테두리
        if (x == 0 || x == width - 1 || y == 0)
        {
            if (isDungeonEntrance && x == width / 2 && y == 0)
            {
                return entranceTile;
            }
            return tileDatas[^2].tileData.baseTile;
        }

        // 2. 맨 위 라인
        if (y == height - 1)
        {
            return tileDatas[^1].tileData.baseTile;
        }

        // 3. 내부 영역만 기존 로직
        float randNum = Random.Range(1, 101);

        if (oreTileProb < randNum)
        {
            if (tileDatas[1].prob >= Random.Range(0f, 100f))
                return tileDatas[1].tileData.baseTile;

            emptyTileList.Add(cellPos);
            return tileDatas[0].tileData.baseTile;
        }
        else
        {
            float randOreProb = Random.Range(0f, sumTileProbs[^1]);

            for (int i = 0; i < sumTileProbs.Length; i++)
            {
                if (sumTileProbs[i] < randOreProb) continue;

                TileData tileData = tileDatas[i + 2].tileData;

                GameObject go = Instantiate(
                    orePrefab,
                    tilemap.GetCellCenterWorld(cellPos),
                    Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)),
                    orePool
                );

                go.GetComponent<OreObj>().Init(tileData.oreData);
                posInOre[cellPos] = go;

                return tileData.baseTile;
            }
        }

        return null;
    }

    void GenerateItems(int[] itemCnt, GameObject[] items)
    {
        for (int i = 0; i < bombObjs.Length; i++)
        {
            for (int j = 0; j < itemCnt[i] && emptyTileList.Count > 0; j++)
            {
                int randNum = Random.Range(0, emptyTileList.Count - 1);
                Vector3Int emptyPos = emptyTileList[randNum];
                emptyTileList.RemoveAt(randNum);

                GameObject go = Instantiate(
                    items[i],
                    tilemap.GetCellCenterWorld(emptyPos),
                    Quaternion.identity,
                    itemPool);

                go.GetComponent<Item>().cellPos = emptyPos;

                posInItems[emptyPos] = go;
            }
        }
    }

    void AlignTopCenter()
    {
        Vector3 cellSize = tilemap.transform.localScale;

        float totalWidth = width * cellSize.x;
        float totalHeight = height * cellSize.y;

        float offsetX = -totalWidth / 2f;
        float offsetY = -totalHeight;

        tilemap.transform.position = new Vector3(offsetX, offsetY, 0);
    }

    void SetWallPos()
    {
        leftWall.position = new(tilemap.GetCellCenterWorld(new(0, 0)).x, leftWall.position.y);
        rightWall.position = new(tilemap.GetCellCenterWorld(new(width - 1, 0)).x, rightWall.position.y);
    }
}