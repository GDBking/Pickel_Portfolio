using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoomPanel : MonoBehaviour
{
    public static BoomPanel Instance;

    [SerializeField] BoomData[] boomDatas;
    [SerializeField] Transform content;
    [SerializeField] GameObject itemSlot;
    [SerializeField] TextMeshProUGUI totalQuantityTxt;
    [SerializeField] Image itemImg;
    [SerializeField] Image rangeImg;
    [SerializeField] TextMeshProUGUI curQuantityTxt;
    [SerializeField] Slider slider;
    [SerializeField] ResourceList resourceList;
    [SerializeField] GameObject descUI;

    readonly GameObject[] itemSlots = new GameObject[6];
    [SerializeField] Color clickSlotColor;
    BoomData clickData;
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

        SetTotalQuantityTxt();

        for (int i = 0; i < boomDatas.Length; i++)
        {
            GameObject go = Instantiate(itemSlot, content);
            go.GetComponent<BombItemSlot>().Init(boomDatas[i], i);
            itemSlots[i] = go;
        }
        SetClickItem(boomDatas[0], 0);
    }

    public void SetClickItem(BoomData data, int idx)
    {
        descUI.SetActive(idx >= 3);

        itemSlots[clickIdx].GetComponent<Image>().color = Color.white;
        itemSlots[idx].GetComponent<Image>().color = clickSlotColor;

        clickData = data;
        clickIdx = idx;

        maxQuantity = GetMaxBombQuantity();
        totalQuantity = GetTotalBombQuantity();

        itemImg.sprite = data.bombSprite;
        rangeImg.sprite = data.rangeSprite;
        SetCurQuantityTxt();
        SetTotalQuantityTxt();
        SetSlider(idx);
        resourceList.SetPiece(data.piece, true);
    }

    public void QuantityBtnClick(bool isIncrease)
    {
        if (isIncrease && maxQuantity == totalQuantity) return;
        if (!isIncrease && SaveManager.saveData.village.boom.bombCnt[clickIdx] == 0) return;

        var saveData = SaveManager.saveData.village.boom;
        saveData.bombCnt[clickIdx] += (isIncrease ? 1 : -1);
        totalQuantity += (isIncrease ? 1 : -1);

        UpkeepCost.Instance.SetUpkeepCost(clickData.piece, isIncrease);
        itemSlots[clickIdx].GetComponent<BombItemSlot>().SetCurAmount();
        SetCurQuantityTxt();
        SetTotalQuantityTxt();
        SetSlider(clickIdx);
    }

    public void ResetBtnClick()
    {
        var saveData = SaveManager.saveData.village.boom;
        BoomData captureclickData = clickData;
        int captureClickIdx = clickIdx;
        for (int i = 0; i < saveData.bombCnt.Length; i++)
        {
            if (saveData.bombCnt[i] == 0) continue;

            clickData = boomDatas[i];
            clickIdx = i;

            while (saveData.bombCnt[i] != 0)
            {
                QuantityBtnClick(false);
            }
        }
        clickData = captureclickData;
        clickIdx = captureClickIdx;

        totalQuantity = 0;
    }

    void SetCurQuantityTxt()
    {
        curQuantityTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.CurQuantity)}" +
            $"{SaveManager.saveData.village.boom.bombCnt[clickIdx]}");
    }

    void SetTotalQuantityTxt()
    {
        totalQuantityTxt.SetText(
            $"{UtilManager.Localization_Text(Key_Village.TotalQuantity)}" +
            $"{totalQuantity}/{maxQuantity}");
    }

    int GetTotalBombQuantity()
    {
        var saveData = SaveManager.saveData.village.boom;
        int sum = 0;
        foreach (int cnt in saveData.bombCnt)
        {
            sum += cnt;
        }
        return sum;
    }

    int GetMaxBombQuantity()
    {
        var elderData = SaveManager.saveData.village.elder;
        return (int)(elderData.x * elderData.y * 0.1f);
    }

    int curValue;
    bool isUpdatingSlider;
    void SetSlider(int idx)
    {
        isUpdatingSlider = true;

        int curQuantity = SaveManager.saveData.village.boom.bombCnt[idx];
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
