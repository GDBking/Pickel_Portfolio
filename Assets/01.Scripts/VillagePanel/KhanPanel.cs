using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KhanPanel : MonoBehaviour
{
    [SerializeField] KhanData[] khanDatas;
    [SerializeField] Image beforeImg, afterImg;
    [SerializeField] TextMeshProUGUI beforePower, beforeDurability;
    [SerializeField] TextMeshProUGUI afterPower, afterDurability;
    [SerializeField] ResourceList resourceList;
    [SerializeField] Button upgradeBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] ParticleSystem upgradeEffect;
    [SerializeField] AudioClip upgradeSound;

    KhanData upgradeData;

    void SetActive()
    {
        var saveData = SaveManager.saveData.village.khan;
        KhanData curData = khanDatas[saveData.level];
        upgradeData = khanDatas[saveData.level + 1];

        beforeImg.sprite = curData.spr;
        afterImg.sprite = upgradeData.spr;

        beforePower.SetText(curData.power.ToString());
        beforeDurability.SetText(curData.durability.ToString());
        afterPower.SetText(upgradeData.power == 0 ? "Max" : upgradeData.power.ToString());
        afterDurability.SetText(upgradeData.durability == 0 ? "Max" : upgradeData.durability.ToString());

        bool isBuy = resourceList.SetPiece(upgradeData.piece);
        upgradeBtn.interactable = isBuy;
        exitBtn.interactable = true;
    }

    public void BuyBtnClick()
    {
        VillageManager.Instance.isChangeScene = false;
        exitBtn.interactable = false;
        upgradeBtn.interactable = false;
        resourceList.BuyBtnClick(upgradeData.piece);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        StartCoroutine(UpgradeCompleted());
    }

    readonly WaitForSeconds wait = new(1f);
    IEnumerator UpgradeCompleted()
    {
        yield return UtilManager.completedWait;

        var saveData = SaveManager.saveData.village.khan;
        saveData.level++;
        saveData.power = upgradeData.power;
        saveData.durability = upgradeData.durability;

        AudioManager.Instance.PlaySfx(upgradeSound);
        upgradeEffect.Play();
        yield return wait;
        SetActive();

        UpkeepCost.Instance.Init();

        VillageManager.Instance.isChangeScene = true;

        SaveManager.Save();
    }

    private void OnEnable()
    {
        SetActive();
    }
}
