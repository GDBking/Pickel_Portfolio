using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BombItemSlot : MonoBehaviour
{
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI curAmountTxt;
    BoomData data;
    int idx;

    public void Init(BoomData data, int idx)
    {
        this.data = data;
        this.idx = idx;
        itemImg.sprite = data.bombSprite;
        SetCurAmount();
    }

    public void OnClick()
    {
        BoomPanel.Instance.SetClickItem(data, idx);
    }

    public void SetCurAmount()
    {
        var saveData = SaveManager.saveData.village.boom;
        curAmountTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.CurQuantity)}" +
            $"{saveData.bombCnt[idx]}");
    }
}
