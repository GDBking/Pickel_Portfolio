using UnityEngine;

public class UpkeepCost : ResourceList
{
    public static UpkeepCost Instance;

    [HideInInspector] public bool isBuy = true;
    [HideInInspector] public bool isNull = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        isBuy = true;
        isNull = true;

        int[] upkeepCost = SaveManager.saveData.orePiece.upkeepCost;
        int[] bagPiece = SaveManager.saveData.orePiece.pieceCnt;
        for (int i = 0; i < upkeepCost.Length; i++)
        {
            amountTexts[i].SetText(upkeepCost[i].ToString());
            if (upkeepCost[i] <= bagPiece[i])
            {
                amountTexts[i].color = Color.green;
            }
            else
            {
                isBuy = false;
                amountTexts[i].color = Color.red;
            }

            if (upkeepCost[i] > 0)
            {
                resources[i].SetActive(true);
                isNull = false;
            }
            else
            {
                resources[i].SetActive(false);
            }
        }
        gameObject.SetActive(!isNull);
    }

    public void SetUpkeepCost(OrePieceData data, bool isIncrease)
    {
        isBuy = true;
        var saveData = SaveManager.saveData.orePiece;

        foreach (var piece in data.orePieces)
        {
            var amountTxt = amountTexts[(int)piece.type];
            int amount = int.Parse(amountTxt.text) + piece.amount * (isIncrease ? 1 : -1);
            saveData.upkeepCost[(int)piece.type] = amount;
            amountTxt.SetText(amount.ToString());
            if (amount <= saveData.pieceCnt[(int)piece.type])
            {
                amountTxt.color = Color.green;
            }
            else
            {
                amountTxt.color = Color.red;
                isBuy = false;
            }

            if (int.Parse(amountTxt.text) > 0)
            {
                resources[(int)piece.type].SetActive(true);
            }
            else
            {
                resources[(int)piece.type].SetActive(false);
            }
        }

        isNull = true;
        foreach (var amount in amountTexts)
        {
            if (int.Parse(amount.text) > 0)
            {
                isNull = false;
                break;
            }
        }
        gameObject.SetActive(!isNull);
    }

    public bool ChangeMiningScene()
    {
        bool isPiece = false;
        int[] upkeepCost = SaveManager.saveData.orePiece.upkeepCost;
        for (int i = 0; i < upkeepCost.Length; i++)
        {
            if (upkeepCost[i] == 0) continue;

            TotalBag.Instance.bags[i].GetComponent<VillageBag>()
                .GivePiece(upkeepCost[i], amountTexts[i], this);

            isPiece = true;

            StartCoroutine(VillageManager.Instance.PlayBuySfx());
            PlayParticle(i, upkeepCost[i]);
        }
        return isPiece;
    }
}
