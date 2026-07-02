using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ResourceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int oreIdx;
    [SerializeField] GameObject infoTxtPrefab;

    public static TextMeshProUGUI infoTxt;

    private void Awake()
    {
        if (infoTxt != null) return;

        Transform canvas = GameObject.FindGameObjectWithTag("GameController").transform;
        infoTxt = Instantiate(infoTxtPrefab, canvas).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string info = UtilManager.Localization_Text(Key_Village.Ore0 + oreIdx);
        infoTxt.SetText(info);
        infoTxt.GetComponent<RectTransform>().position = (Vector2)GetComponent<RectTransform>().position + Vector2.up * 0.5f;
        infoTxt.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoTxt.gameObject.SetActive(false);
    }
}
