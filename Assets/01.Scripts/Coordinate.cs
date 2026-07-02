using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Coordinate : MonoBehaviour
{
    TextMeshProUGUI coordinateTxt;
    Tilemap tilemap;
    Transform playerTransform;

    int heightOffset;

    private void Awake()
    {
        coordinateTxt = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        tilemap = TileManager.Instance.tilemap;
        playerTransform = PlayerDig.Instance.transform;

        heightOffset = SaveManager.saveData.village.elder.y + 1;
    }

    private void Update()
    {
        Vector3Int cellPos = tilemap.WorldToCell(playerTransform.position);
        coordinateTxt.SetText($"X: {cellPos.x}  /  Y: {cellPos.y - heightOffset}");
    }
}
