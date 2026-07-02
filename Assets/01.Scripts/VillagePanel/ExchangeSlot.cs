using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ExchangeSlot : MonoBehaviour
{
    private static readonly int GetHash = Animator.StringToHash("Get");
    private static readonly int IsSoldOutHash = Animator.StringToHash("IsSoldOut");
    [SerializeField] Image giveOreImg;
    [SerializeField] Image getOreImg;
    [SerializeField] TextMeshProUGUI giveOreTxt;
    [SerializeField] TextMeshProUGUI getOreTxt;
    [SerializeField] Animator giveOreAnim;
    [SerializeField] Animator getOreAnim;
    public Animator anim;
    [SerializeField] int slotIdx;
    [SerializeField] ParticleSystem giveOreParticle;
    [SerializeField] ParticleSystem getOreParticle;
    [SerializeField] Material[] orePieceMats;

    SaveManager.Crow saveData;

    public void Init(bool isFirst)
    {
        anim.SetBool(IsSoldOutHash, false);

        int[] orePieceCnts = SaveManager.saveData.orePiece.pieceCnt;
        saveData = SaveManager.saveData.village.crow;
        List<int> existIdxList = new();
        for (int i = 0; i < orePieceCnts.Length; i++)
        {
            if (orePieceCnts[i] > 0)
                existIdxList.Add(i);
        }

        if (existIdxList.Count == 0)
        {
            saveData.isActive[slotIdx] = false;
            gameObject.SetActive(false);
            return;
        }

        if (isFirst)
        {
            if (existIdxList.Count != 1 || existIdxList[0] != 0)
            {
                saveData.giveOreIdx[slotIdx] = existIdxList[Random.Range(existIdxList[0] == 0 ? 1 : 0, existIdxList.Count)];
            }
            else
            {
                saveData.giveOreIdx[slotIdx] = existIdxList[Random.Range(0, existIdxList.Count)];
            }
        }
        else
        {
            saveData.giveOreIdx[slotIdx] = existIdxList[Random.Range(0, existIdxList.Count)];
        }

        int unlockOreIdx = (int)SaveManager.saveData.village.elder.oreIdx + 1;
        do
        {
            saveData.getOreIdx[slotIdx] = isFirst ? 0 : Random.Range(0, unlockOreIdx);
        }
        while (saveData.giveOreIdx[slotIdx] == saveData.getOreIdx[slotIdx]);

        if (saveData.giveOreIdx[slotIdx] < saveData.getOreIdx[slotIdx])
        {
            float ratio = 10f;
            for (int i = saveData.giveOreIdx[slotIdx]; i < saveData.getOreIdx[slotIdx]; i++)
            {
                ratio *= CrowPanel.Instance.oreRatio[i];
            }
            ratio = (ratio - 10f) * 1.5f + 10f;

            if (orePieceCnts[saveData.giveOreIdx[slotIdx]] < ratio)
            {
                saveData.isActive[slotIdx] = false;
                gameObject.SetActive(false);
                return;
            }

            saveData.giveOreQuantity[slotIdx] = Mathf.RoundToInt(Random.Range(1, (int)(orePieceCnts[saveData.giveOreIdx[slotIdx]] / ratio + 1)) * ratio);
            saveData.getOreQuantity[slotIdx] = Mathf.RoundToInt(saveData.giveOreQuantity[slotIdx] / ratio * 10);
        }
        else
        {
            if (orePieceCnts[saveData.giveOreIdx[slotIdx]] < 10)
            {
                saveData.isActive[slotIdx] = false;
                gameObject.SetActive(false);
                return;
            }

            float ratio = 10f;
            for (int i = saveData.getOreIdx[slotIdx]; i < saveData.giveOreIdx[slotIdx]; i++)
            {
                ratio *= CrowPanel.Instance.oreRatio[i];
            }
            ratio = (ratio - 10f) / 1.5f + 10f;

            saveData.giveOreQuantity[slotIdx] = Random.Range(1, orePieceCnts[saveData.giveOreIdx[slotIdx]] / 10 + 1) * 10;
            saveData.getOreQuantity[slotIdx] = Mathf.RoundToInt(saveData.giveOreQuantity[slotIdx] / 10 * ratio);
        }

        SetUI();
    }

    public void SetLoad()
    {
        saveData = SaveManager.saveData.village.crow;
        if (!saveData.isActive[slotIdx])
        {
            gameObject.SetActive(false);
            return;
        }

        SetUI();

        if (saveData.isSoldOut[slotIdx])
            anim.SetBool(IsSoldOutHash, true);
    }

    void SetUI()
    {
        giveOreImg.sprite = CrowPanel.Instance.oreSprites[saveData.giveOreIdx[slotIdx]];
        getOreImg.sprite = CrowPanel.Instance.oreSprites[saveData.getOreIdx[slotIdx]];
        giveOreTxt.SetText($"X{saveData.giveOreQuantity[slotIdx]}");
        getOreTxt.SetText($"X{saveData.getOreQuantity[slotIdx]}");
    }

    public void ExchangeBtnClick()
    {
        int[] pieceCnts = SaveManager.saveData.orePiece.pieceCnt;
        if (saveData.giveOreQuantity[slotIdx] > pieceCnts[saveData.giveOreIdx[slotIdx]]) return;

        VillageManager.Instance.blockPanel.SetActive(true);
        VillageManager.Instance.isChangeScene = false;
        StartCoroutine(ExchangeBtnClickRoutine());
    }

    IEnumerator ExchangeBtnClickRoutine()
    {
        TotalBag.Instance.bags[saveData.giveOreIdx[slotIdx]].GetComponent<VillageBag>()
            .ExchangePiece(saveData.giveOreQuantity[slotIdx], true, this);

        StartParticle(giveOreParticle, saveData.giveOreIdx[slotIdx], saveData.giveOreQuantity[slotIdx]);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        yield return UtilManager.completedWait;

        TotalBag.Instance.bags[saveData.getOreIdx[slotIdx]].GetComponent<VillageBag>()
            .ExchangePiece(saveData.getOreQuantity[slotIdx], false, this);

        StartParticle(getOreParticle, saveData.getOreIdx[slotIdx], saveData.getOreQuantity[slotIdx]);

        StartCoroutine(VillageManager.Instance.PlayBuySfx());
        yield return UtilManager.completedWait;

        anim.SetBool(IsSoldOutHash, true);
        saveData.isSoldOut[slotIdx] = true;

        VillageManager.Instance.blockPanel.SetActive(false);
        VillageManager.Instance.isChangeScene = true;

        UpkeepCost.Instance.Init();

        SaveManager.Save();

        ResourcePin.Instance.SetPiece(ResourcePin.Instance.LoadPinData(SaveManager.saveData.pinData));
    }

    void StartParticle(ParticleSystem ps, int oreIdx, int quantity)
    {
        ps.GetComponent<ParticleSystemRenderer>().material = orePieceMats[oreIdx];

        var emission = ps.emission;
        emission.rateOverTime = Mathf.Clamp(quantity / 5, 2, 50);

        var subEmitters = ps.subEmitters;
        subEmitters.RemoveSubEmitter(0);
        subEmitters.AddSubEmitter(
                ps.transform.GetChild(oreIdx).GetComponent<ParticleSystem>(),
                ParticleSystemSubEmitterType.Birth,
                ParticleSystemSubEmitterProperties.InheritNothing, 0.5f);

        ps.Play();
    }

    public void ExchangeAnim(bool isGive)
    {
        Animator anim = isGive ? giveOreAnim : getOreAnim;
        anim.SetTrigger(GetHash);
    }

    public void ExchangeCntTxt(bool isGive)
    {
        TextMeshProUGUI text = isGive ? giveOreTxt : getOreTxt;
        text.SetText($"X{(isGive ? --saveData.giveOreQuantity[slotIdx] : --saveData.getOreQuantity[slotIdx])}");
    }
}
