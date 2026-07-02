using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LunaDatas
{
    public LunaData[] lunaDatas;
}

public class LunaPanel : MonoBehaviour
{
    [SerializeField] LunaDatas[] lunaDataArr;
    [SerializeField] Button[] upgradeBtns;
    [SerializeField] TextMeshProUGUI desc;
    [SerializeField] ResourceList resourceList;
    [SerializeField] ParticleSystem[] upgradeEffects;
    [SerializeField] AudioClip upgradeSound;
    [SerializeField] AudioClip upgradeCompletedSound;

    LunaData upgradeData;
    bool isMax;

    void SetActive()
    {
        desc.SetText("");
        for (int i = 2; i >= 0; i--)
            BtnHover(i);
    }

    public void BtnHover(int idx)
    {
        var saveData = SaveManager.saveData.village.luna;
        LunaData curData = lunaDataArr[idx].lunaDatas[saveData.level[idx]];
        upgradeData = lunaDataArr[idx].lunaDatas[saveData.level[idx] + 1];

        isMax = upgradeData.piece == null;

        if (idx == 0)
        {
            string text = UtilManager.Localization_Text(Key_Village.OreProb);
            if (!isMax)
                desc.SetText($"{text}\n<color=red>{curData.min}%</color> => <color=green>{upgradeData.min}%</color>");
            else
                desc.SetText($"{text}\nMax Level: <color=green>{curData.min}%</color>");
        }
        else if (idx == 1)
        {
            string text = UtilManager.Localization_Text(Key_Village.OrePiece);
            if (!isMax)
            {
                desc.SetText($"{text}\n" +
                    $"Min: <color=red>{curData.min}</color> => <color=green>{upgradeData.min}</color>\n" +
                    $"Max: <color=red>{curData.max}</color> => <color=green>{upgradeData.max}</color>");
            }
            else
            {
                desc.SetText($"{text}\n" +
                    $"Max Level\n" +
                    $"Min: <color=green>{curData.min}</color>\n" +
                    $"Max: <color=green>{curData.max}</color>");
            }
        }
        else
        {
            string text = UtilManager.Localization_Text(Key_Village.Obstacle);
            if (!isMax)
                desc.SetText($"{text}\n<color=red>{curData.min}%</color> => <color=green>{upgradeData.min}%</color>");
            else
                desc.SetText($"{text}\nMax Level: <color=green>{curData.min}%</color>");
        }

        upgradeBtns[idx].interactable = resourceList.SetPiece(upgradeData.piece);
    }

    public void UpgradeBtnClick(int idx)
    {
        if (isMax) return;

        VillageManager.Instance.isChangeScene = false;
        VillageManager.Instance.blockPanel.SetActive(true);
        resourceList.BuyBtnClick(upgradeData.piece);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        StartCoroutine(UpgradeCompleted(idx));
    }

    readonly WaitForSeconds wait = new(2.2f);
    IEnumerator UpgradeCompleted(int idx)
    {
        yield return UtilManager.completedWait;

        var saveData = SaveManager.saveData.village.luna;
        saveData.level[idx]++;

        switch (idx)
        {
            case 0: 
                saveData.oreProb = upgradeData.min;
                break;
            case 1:
                saveData.orePieceMin = upgradeData.min;
                saveData.orePieceMax = upgradeData.max;
                break;
            case 2:
                saveData.obstacleProb = upgradeData.min;
                break;
        }

        AudioManager.Instance.PlaySfx(upgradeSound);
        upgradeEffects[idx].Play();
        yield return wait;
        AudioManager.Instance.PlaySfx(upgradeCompletedSound);
        BtnHover(idx);

        UpkeepCost.Instance.Init();

        VillageManager.Instance.isChangeScene = true;
        VillageManager.Instance.blockPanel.SetActive(false);

        SaveManager.Save();
    }

    private void OnEnable()
    {
        SetActive();
    }
}
