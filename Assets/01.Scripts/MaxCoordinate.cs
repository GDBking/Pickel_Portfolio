using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MaxCoordinate : MonoBehaviour
{
    TextMeshProUGUI maxCoordinateTxt;

    private void Awake()
    {
        maxCoordinateTxt = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        var saveData = SaveManager.saveData.village.elder;
        int x = saveData.x;
        int y = saveData.y;

        maxCoordinateTxt.SetText($"X: {x}  /  Y: {-y}");
    }
}
