using TMPro;
using UnityEngine;

public class TotalPanelUpkeepCost : MonoBehaviour
{
    [SerializeField] GameObject[] resources;
    [SerializeField] TextMeshProUGUI[] quantityTxts;

    public void Init()
    {
        bool isNull = true;

        int[] savePieces = SaveManager.saveData.orePiece.pieceCnt;
        int[] upkeepCost = SaveManager.saveData.orePiece.upkeepCost;
        MineBag[] mineBags = TileManager.Instance.bags;
        for (int i = 0; i < upkeepCost.Length; i++)
        {
            quantityTxts[i].SetText(upkeepCost[i].ToString());
            if (upkeepCost[i] <= savePieces[i] + mineBags[i].quantity)
            {
                quantityTxts[i].color = Color.green;
            }
            else
            {
                quantityTxts[i].color = Color.red;
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
}
