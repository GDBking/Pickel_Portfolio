using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElderPanel : MonoBehaviour
{
    [SerializeField] ElderData[] elderDatas;
    [SerializeField] Image beforeImg, afterImg;
    [SerializeField] TextMeshProUGUI beforeX, beforeY, afterX, afterY;
    [SerializeField] ResourceList resourceList;
    [SerializeField] Image npcImg;
    [SerializeField] List<Sprite> npcSprs;
    [SerializeField] Button buyBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] ParticleSystem upgradeEffect;
    [SerializeField] AudioClip upgradeSound;

    ElderData upgradeData;

    void SetActive()
    {
        var saveData = SaveManager.saveData.village.elder;
        ElderData curData = elderDatas[saveData.level];
        upgradeData = elderDatas[saveData.level + 1];

        beforeImg.sprite = curData.spr;
        afterImg.sprite = upgradeData.spr;

        beforeX.SetText(curData.x.ToString());
        beforeY.SetText(curData.y.ToString());
        afterX.SetText(upgradeData.x == 0 ? "Max" : upgradeData.x.ToString());
        afterY.SetText(upgradeData.y == 0 ? "Max" : upgradeData.y.ToString());

        bool isUnlock = SaveManager.saveData.npc.isUnlock;
        if (saveData.level == 0 || upgradeData.x == 0)
        {
            npcImg.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            npcImg.transform.parent.gameObject.SetActive(true);
            npcImg.color = isUnlock ? Color.black : Color.white;
            npcImg.sprite = npcSprs[saveData.level - 1];
        }

        bool isBuy = resourceList.SetPiece(upgradeData.piece) && !isUnlock;
        buyBtn.interactable = isBuy;
        exitBtn.interactable = true;
    }

    public void BuyBtnClick()
    {
        VillageManager.Instance.isChangeScene = false;
        exitBtn.interactable = false;
        buyBtn.interactable = false;
        resourceList.BuyBtnClick(upgradeData.piece);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        StartCoroutine(BuyCompleted());
    }

    readonly WaitForSeconds wait = new(1f);
    IEnumerator BuyCompleted()
    {
        yield return UtilManager.completedWait;

        var saveData = SaveManager.saveData.village.elder;
        saveData.level++;
        saveData.x = upgradeData.x;
        saveData.y = upgradeData.y;
        saveData.baseTileHP = upgradeData.baseTileHP;
        saveData.oreIdx = upgradeData.oreIdx;
        SaveManager.saveData.npc.isUnlock = true;

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
