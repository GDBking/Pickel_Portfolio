using UnityEngine;
using static SaveManager;

public class ResourcePin : ResourceList
{
    public static ResourcePin Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetPiece(LoadPinData(SaveManager.saveData.pinData));
    }

    public OrePieceSaveData SavePinData(OrePieceData data)
    {
        OrePieceSaveData saveData = new()
        {
            orePieces = new OrePieceSaveData.OrePieceInfo[data.orePieces.Length]
        };

        for (int i = 0; i < data.orePieces.Length; i++)
        {
            saveData.orePieces[i] = new OrePieceSaveData.OrePieceInfo
            {
                type = data.orePieces[i].type,
                amount = data.orePieces[i].amount
            };
        }

        return saveData;
    }

    public OrePieceData LoadPinData(OrePieceSaveData saveData)
    {
        if (saveData == null)
            return null;

        OrePieceData data = ScriptableObject.CreateInstance<OrePieceData>();

        data.orePieces = new OrePieceData.OrePiece[saveData.orePieces.Length];

        for (int i = 0; i < saveData.orePieces.Length; i++)
        {
            data.orePieces[i] = new OrePieceData.OrePiece
            {
                type = saveData.orePieces[i].type,
                amount = saveData.orePieces[i].amount
            };
        }

        return data;
    }

    public void PinBtnClick2()
    {
        gameObject.SetActive(false);
        SaveManager.saveData.pinData = null;

        SaveManager.Save();
    }
}
