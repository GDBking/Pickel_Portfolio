using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoraPanel : MonoBehaviour
{
    public static PoraPanel Instance;

    [SerializeField] PoraData[] poraDatas;
    [SerializeField] Transform content;
    [SerializeField] GameObject itemSlot;
    [SerializeField] TextMeshProUGUI totalQuantityTxt;
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI desc;
    [SerializeField] TextMeshProUGUI curQuantityTxt;
    [SerializeField] Slider slider;
    [SerializeField] ResourceList resourceList;

    readonly GameObject[] itemSlots = new GameObject[6];
    [SerializeField] Color clickSlotColor;
    PoraData clickData;
    int clickIdx;

    int maxQuantity;
    int totalQuantity;
    private void Awake()
    {
        Instance = this;
    }

    bool isInit;
    void SetActive()
    {
        if (isInit) return;
        isInit = true;

        SetTotalQuantity();

        for (int i = 0; i < poraDatas.Length; i++)
        {
            GameObject go = Instantiate(itemSlot, content);
            go.GetComponent<PotionItemSlot>().Init(poraDatas[i], i);
            itemSlots[i] = go;
        }
        SetClickItem(poraDatas[0], 0);
    }

    public void SetClickItem(PoraData data, int idx)
    {
        itemSlots[clickIdx].GetComponent<Image>().color = Color.white;
        itemSlots[idx].GetComponent<Image>().color = clickSlotColor;

        clickData = data;
        clickIdx = idx;

        maxQuantity = GetMaxPotionQuantity();
        totalQuantity = GetTotalPotionQuantity();

        itemImg.sprite = data.potionSprite;
        desc.SetText(UtilManager.Localization_Text(data.key));
        SetCurQuantity();
        SetTotalQuantity();
        SetSlider(idx);
        resourceList.SetPiece(data.piece, true);
    }

    public void QuantityBtnClick(bool isIncrease)
    {
        if (isIncrease && maxQuantity == totalQuantity) return;
        if (!isIncrease && SaveManager.saveData.village.pora.potionCnt[clickIdx] == 0) return;

        var saveData = SaveManager.saveData.village.pora;
        saveData.potionCnt[clickIdx] += (isIncrease ? 1 : -1);
        totalQuantity += (isIncrease ? 1 : -1);

        UpkeepCost.Instance.SetUpkeepCost(clickData.piece, isIncrease);
        itemSlots[clickIdx].GetComponent<PotionItemSlot>().SetCurAmount();
        SetCurQuantity();
        SetTotalQuantity();
        SetSlider(clickIdx);
    }

    public void ResetBtnClick()
    {
        var saveData = SaveManager.saveData.village.pora;
        PoraData captureclickData = clickData;
        int captureClickIdx = clickIdx;
        for (int i = 0; i < saveData.potionCnt.Length; i++)
        {
            if (saveData.potionCnt[i] == 0) continue;

            clickData = poraDatas[i];
            clickIdx = i;

            while (saveData.potionCnt[i] != 0)
            {
                QuantityBtnClick(false);
            }
        }
        clickData = captureclickData;
        clickIdx = captureClickIdx;

        totalQuantity = 0;
    }

    void SetCurQuantity()
    {
        curQuantityTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.CurQuantity)}" +
            $"{SaveManager.saveData.village.pora.potionCnt[clickIdx]}");
    }

    void SetTotalQuantity()
    {
        totalQuantityTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.TotalQuantity)}" +
            $"{totalQuantity}/{maxQuantity}");
    }

    int GetTotalPotionQuantity()
    {
        var saveData = SaveManager.saveData.village.pora;
        int sum = 0;
        foreach (int cnt in saveData.potionCnt)
        {
            sum += cnt;
        }
        return sum;
    }

    int GetMaxPotionQuantity()
    {
        var elderData = SaveManager.saveData.village.elder;
        return (int)(elderData.x * elderData.y * 0.05f);
    }

    int curValue;
    bool isUpdatingSlider;
    void SetSlider(int idx)
    {
        isUpdatingSlider = true;
        
        int curQuantity = SaveManager.saveData.village.pora.potionCnt[idx];
        slider.maxValue = maxQuantity - totalQuantity + curQuantity;
        slider.SetValueWithoutNotify(curQuantity);
        curValue = (int)slider.value;

        isUpdatingSlider = false;
    }

    public void OnValueChanged()
    {
        if (isUpdatingSlider) return;

        int newValue = (int)slider.value;
        int diff = newValue - curValue;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
                QuantityBtnClick(true);
        }
        else if (diff < 0)
        {
            for (int i = 0; i < -diff; i++)
                QuantityBtnClick(false);
        }

        curValue = newValue;
    }

    private void OnEnable()
    {
        SetActive();
    }

    private void OnDisable()
    {
        SaveManager.Save();
    }
}
