using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Pickax : MonoBehaviour
{
    public static Pickax Instance;

    [SerializeField] Sprite[] pickaxSprites;
    [SerializeField] Image durabilityGauge;
    [SerializeField] TextMeshProUGUI durabilityTxt;

    SpriteRenderer sprRen;
    [HideInInspector] public int durability;
    private int curDurability;

    public int CurDurability { get => curDurability; set => curDurability = Mathf.Min(value, durability); }

    private void Awake()
    {
        Instance = this;

        sprRen = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var saveData = SaveManager.saveData.village.khan;
        sprRen.sprite = pickaxSprites[saveData.level];
        PlayerDig.Instance.digPower = saveData.power;
        durability = saveData.durability;
        CurDurability = durability;
        durabilityTxt.SetText(CurDurability.ToString());

        gameObject.SetActive(false);
    }

    bool isWarnning;
    public void SetDurability(int amount)
    {
        if (PlayerDig.Instance.isPowerUp && amount < 0)
        {
            return;
        }

        CurDurability += amount;
        durabilityGauge.fillAmount = CurDurability / (float)durability;
        durabilityTxt.SetText(CurDurability.ToString());

        if (!isWarnning && durabilityGauge.fillAmount <= 0.1f)
        {
            isWarnning = true;
            durabilityTxt.color = Color.red;
        }
        if (isWarnning && durabilityGauge.fillAmount > 0.1f)
        {
            isWarnning = false;
            durabilityTxt.color = Color.black;
        }
    }
}
