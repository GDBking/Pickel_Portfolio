using System.Collections;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public static VillageManager Instance;

    public GameObject[] npcList;
    public GameObject[] buildingList;
    [SerializeField] GameObject buildEffect;
    [SerializeField] Material defaultMat;
    [SerializeField] Material outlineMat;

    [HideInInspector] public bool isChangeScene = true;

    [Header("Instance")]
    [SerializeField] BuildingPanel buildingPanel;

    [Header("Block Panel")]
    public GameObject blockPanel;

    [Header("Sound")]
    [SerializeField] AudioClip buildSound;
    [SerializeField] AudioClip buildCompletedSound;
    [SerializeField] AudioClip buySound;

    private void Awake()
    {
        Instance = this;
        BuildingPanel.Instance = buildingPanel;
    }

    private void Start()
    {
        SetVillage();
    }

    void SetVillage()
    {
        for (int i = 0; i < SaveManager.saveData.npc.unlockIdx && i < npcList.Length; i++)
        {
            npcList[i].SetActive(true);
        }

        for (int i = 0; i < buildingList.Length; i++)
        {
            buildingList[i].SetActive(SaveManager.saveData.village.isUnlock[i]);
        }
    }

    public void BuildBuilding(int buildingIdx)
    {
        ResourceInfo.infoTxt.gameObject.SetActive(false);
        StartCoroutine(BuildBuildingRoutine(buildingIdx));
    }

    readonly WaitForSeconds wait = new(2.5f);
    IEnumerator BuildBuildingRoutine(int buildingIdx)
    {
        AudioManager.Instance.PlaySfx(buildSound);

        SaveManager.saveData.village.isUnlock[buildingIdx] = true;
        GameObject go = buildingList[buildingIdx];

        Collider2D col = go.GetComponent<Collider2D>();
        col.enabled = false;

        isChangeScene = true;

        Instantiate(buildEffect, go.transform.position, Quaternion.identity);

        yield return wait;

        AudioManager.Instance.PlaySfx(buildCompletedSound);

        UpkeepCost.Instance.Init();
        ResourcePin.Instance.SetPiece(ResourcePin.Instance.LoadPinData(SaveManager.saveData.pinData));

        go.SetActive(true);
        col.enabled = true;
    }

    public void SetMogPanel(bool isEnable)
    {
        if (!isEnable && ResourceInfo.infoTxt != null)
        {
            ResourceInfo.infoTxt.gameObject.SetActive(false);
        }

        foreach (GameObject npc in npcList)
        {
            if (npc == null || !npc.activeSelf) continue;

            npc.GetComponent<SpriteRenderer>().material = isEnable ? defaultMat : outlineMat;
        }

        foreach (GameObject building in buildingList)
        {
            if (building == null || !building.activeSelf) continue;

            building.GetComponent<SpriteRenderer>().material = isEnable ? defaultMat : outlineMat;
        }
    }

    public IEnumerator PlayBuySfx()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1.7f)
        {
            AudioManager.Instance.PlaySfx(buySound);

            float randWait = Random.Range(0.1f, 0.2f);
            yield return new WaitForSeconds(randWait);
            elapsedTime += randWait;
        }
    }
}
