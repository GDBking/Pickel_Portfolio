using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VillageBag : MonoBehaviour
{
    private static readonly int GetHash = Animator.StringToHash("Get");
    int idx;
    Animator anim;
    TextMeshProUGUI pieceCntTxt;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pieceCntTxt = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetCount(int idx)
    {
        this.idx = idx;
        pieceCntTxt.SetText(SaveManager.saveData.orePiece.pieceCnt[idx].ToString());

        if (SaveManager.saveData.orePiece.pieceCnt[idx] == 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void GivePiece(int quantity, TextMeshProUGUI cntTxt, ResourceList resourceList)
    {
        for (int i = 0; i < quantity; i++)
        {
            StartCoroutine(GivePieceRoutine(cntTxt, resourceList));
        }
        SaveManager.saveData.orePiece.pieceCnt[idx] -= quantity;
    }

    IEnumerator GivePieceRoutine(TextMeshProUGUI cntTxt, ResourceList resourceList)
    {
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Bag_In")
        {
            anim.SetTrigger(GetHash);
            resourceList.GetPieceAnim(idx);
        }

        pieceCntTxt.SetText((int.Parse(pieceCntTxt.text) - 1).ToString());
        cntTxt.SetText((int.Parse(cntTxt.text) - 1).ToString());
    }

    public void ExchangePiece(int quantity, bool isGive, ExchangeSlot exchangeSlot)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < quantity; i++)
        {
            StartCoroutine(ExchangePieceRoutine(isGive, exchangeSlot));
        }
        SaveManager.saveData.orePiece.pieceCnt[idx] += quantity * (isGive ? -1 : 1);
    }

    IEnumerator ExchangePieceRoutine(bool isGive, ExchangeSlot exchangeSlot)
    {
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Bag_In")
        {
            anim.SetTrigger(GetHash);
            exchangeSlot.ExchangeAnim(isGive);
        }

        pieceCntTxt.SetText((int.Parse(pieceCntTxt.text) + 1 * (isGive ? -1 : 1)).ToString());
        exchangeSlot.ExchangeCntTxt(isGive);
    }
}
