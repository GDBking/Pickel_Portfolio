using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PotionItemSlot : MonoBehaviour
{
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI curAmountTxt;
    PoraData data;
    int idx;

    public void Init(PoraData data, int idx)
    {
        this.data = data;
        this.idx = idx;
        itemImg.sprite = data.potionSprite;
        SetCurAmount();
    }

    public void OnClick()
    {
        PoraPanel.Instance.SetClickItem(data, idx);
    }

    public void SetCurAmount()
    {
        var saveData = SaveManager.saveData.village.pora;
        curAmountTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.CurQuantity)}" +
            $"{saveData.potionCnt[idx]}");
    }
}
