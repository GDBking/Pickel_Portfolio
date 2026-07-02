using UnityEngine;

public class CrowPanel : MonoBehaviour
{
    public static CrowPanel Instance;

    [SerializeField] ExchangeSlot[] exchangeSlots;
    public Sprite[] oreSprites;
    public float[] oreRatio;

    private void Awake()
    {
        Instance = this;
    }

    void SetActive()
    {
        var saveData = SaveManager.saveData.village.crow;
        if (!saveData.isSetting)
        {
            SaveManager.saveData.village.crow = new()
            {
                isSetting = true
            };
            for (int i = 0; i < exchangeSlots.Length; i++)
            {
                exchangeSlots[i].Init(i == 0);
            }
        }
        else
        {
            foreach (ExchangeSlot slot in exchangeSlots)
                slot.SetLoad();
        }
    }

    private void OnEnable()
    {
        SetActive();
    }
}
