using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    public static BuildingPanel Instance;

    [SerializeField] Image imgComp;
    [SerializeField] TextMeshProUGUI nameTxtComp;
    [SerializeField] TextMeshProUGUI descTxtComp;
    [SerializeField] ResourceList resourceList;
    [SerializeField] Button buildBtn;
    [SerializeField] Button exitBtn;

    OrePieceData data;
    int buildingIdx;

    public void Init(Sprite sprite, string name, string desc, OrePieceData data, int buildingIdx)
    {
        imgComp.sprite = sprite;
        nameTxtComp.SetText(name);
        descTxtComp.SetText(desc);

        bool isBuy = resourceList.SetPiece(data);
        exitBtn.interactable = true;
        buildBtn.interactable = isBuy;

        this.data = data;
        this.buildingIdx = buildingIdx;

        gameObject.SetActive(true);
    }

    public void BuildBtnClick()
    {
        VillageManager.Instance.isChangeScene = false;
        exitBtn.interactable = false;
        buildBtn.interactable = false;
        resourceList.BuyBtnClick(data);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        StartCoroutine(BuyCompleted());
    }

    IEnumerator BuyCompleted()
    {
        yield return UtilManager.completedWait;

        VillageManager.Instance.BuildBuilding(buildingIdx);
        gameObject.SetActive(false);

        SaveManager.Save();
    }
}
