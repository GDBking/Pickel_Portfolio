using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MogPanel : MonoBehaviour
{
    [SerializeField] MogData[] mogDatas;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI twinkleTxt;
    [SerializeField] Button upgradeBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] ResourceList resourceList;
    [SerializeField] Light2D globalLight;
    [SerializeField] Light2D playerLight;
    [SerializeField] AudioClip upgradeSound;

    MogData curData;
    MogData upgradeData;

    void SetActive()
    {
        var saveData = SaveManager.saveData.village.mog;
        curData = mogDatas[saveData.level];
        upgradeData = mogDatas[saveData.level + 1];

        globalLight.intensity = 0f;
        playerLight.pointLightInnerRadius = curData.inner;
        playerLight.pointLightOuterRadius = curData.outer;
        playerLight.enabled = true;

        levelTxt.SetText(saveData.level == mogDatas.Length - 2 ? "Level: Max" : $"Level: {saveData.level}");
        twinkleTxt.SetText($"{UtilManager.Localization_Text(Key_Village.Twinkle)}{curData.twinkleProb}%");

        bool isUpgrade = resourceList.SetPiece(upgradeData.piece);
        upgradeBtn.interactable = isUpgrade;
        exitBtn.interactable = true;
    }

    public void UpgradeBtnClick()
    {
        VillageManager.Instance.isChangeScene = false;
        exitBtn.interactable = false;
        upgradeBtn.interactable = false;
        resourceList.BuyBtnClick(upgradeData.piece);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        StartCoroutine(UpgradeCompleted());
    }

    IEnumerator UpgradeCompleted()
    {
        yield return UtilManager.completedWait;

        var saveData = SaveManager.saveData.village.mog;
        saveData.level++;
        saveData.inner = upgradeData.inner;
        saveData.outer = upgradeData.outer;
        saveData.twinkleProb = upgradeData.twinkleProb;

        AudioManager.Instance.PlaySfx(upgradeSound);
        float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            playerLight.pointLightInnerRadius = Mathf.Lerp(curData.inner, upgradeData.inner, elapsedTime / duration);
            playerLight.pointLightOuterRadius = Mathf.Lerp(curData.outer, upgradeData.outer, elapsedTime / duration);
        }

        SetActive();

        UpkeepCost.Instance.Init();

        VillageManager.Instance.isChangeScene = true;

        SaveManager.Save();
    }

    private void OnEnable()
    {
        UpkeepCost.Instance.gameObject.SetActive(false);
        VillageManager.Instance.SetMogPanel(true);
        SetActive();
    }

    private void OnDisable()
    {
        if (globalLight != null)
            globalLight.intensity = 1f;
        if (playerLight != null)
            playerLight.enabled = false;
        if (UpkeepCost.Instance != null)
            UpkeepCost.Instance.Init();
        if (VillageManager.Instance != null)
            VillageManager.Instance.SetMogPanel(false);
    }
}
