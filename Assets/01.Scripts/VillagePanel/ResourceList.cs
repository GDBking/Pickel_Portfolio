using TMPro;
using UnityEngine;

public class ResourceList : MonoBehaviour
{
    private static readonly int GetHash = Animator.StringToHash("Get");
    [SerializeField] protected GameObject[] resources;
    [SerializeField] protected TextMeshProUGUI[] amountTexts;
    [SerializeField] protected Animator[] anims;
    [SerializeField] protected ParticleSystem[] particles;

    OrePieceData data;

    public bool SetPiece(OrePieceData data, bool isUpkeep = false)
    {
        this.data = data;

        bool isBuy = true;
        foreach (GameObject resource in resources)
            resource.SetActive(false);

        if (data == null || data.orePieces.Length == 0)
        {
            gameObject.SetActive(false);
            return false;
        }

        foreach (var piece in data.orePieces)
        {
            var amountTxt = amountTexts[(int)piece.type];
            amountTxt.SetText(piece.amount.ToString());
            if (!isUpkeep)
            {
                if (piece.amount <= SaveManager.saveData.orePiece.pieceCnt[(int)piece.type])
                {
                    amountTxt.color = Color.green;
                }
                else
                {
                    amountTxt.color = Color.red;
                    isBuy = false;
                }
            }

            resources[(int)piece.type].SetActive(true);
        }
        gameObject.SetActive(true);

        return isBuy;
    }

    public void BuyBtnClick(OrePieceData data)
    {
        foreach (var piece in data.orePieces)
        {
            TotalBag.Instance.bags[(int)piece.type]
                .GetComponent<VillageBag>()
                .GivePiece(piece.amount, amountTexts[(int)piece.type], this);

            PlayParticle((int)piece.type, piece.amount);
        }

        if (IsSameData(data, ResourcePin.Instance.LoadPinData(SaveManager.saveData.pinData)))
            SaveManager.saveData.pinData = null;
        ResourcePin.Instance.SetPiece(ResourcePin.Instance.LoadPinData(SaveManager.saveData.pinData));
    }

    protected void PlayParticle(int idx, int quantity)
    {
        var emission = particles[idx].emission;
        emission.rateOverTime = Mathf.Clamp(quantity / 5, 2, 50);
        
        particles[idx].Play();
    }

    public void GetPieceAnim(int idx) => anims[idx].SetTrigger(GetHash);

    public void PinBtnClick()
    {
        SaveManager.saveData.pinData = ResourcePin.Instance.SavePinData(data);

        ResourcePin.Instance.SetPiece(data);

        ResourcePin.Instance.gameObject.SetActive(true);

        SaveManager.Save();
    }

    bool IsSameData(OrePieceData a, OrePieceData b)
    {
        if (a == null || b == null)
            return false;

        if (a.orePieces.Length != b.orePieces.Length)
            return false;

        for (int i = 0; i < a.orePieces.Length; i++)
        {
            if (a.orePieces[i].type != b.orePieces[i].type)
                return false;

            if (a.orePieces[i].amount != b.orePieces[i].amount)
                return false;
        }

        return true;
    }
}
