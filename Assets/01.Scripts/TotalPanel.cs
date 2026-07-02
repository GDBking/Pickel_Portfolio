using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TotalPanel : MonoBehaviour
{
    public static TotalPanel Instance; // TileManager에서 초기화

    [SerializeField] GameObject[] orePieceSlots;
    [SerializeField] TextMeshProUGUI[] totalQuantityTxts;
    [SerializeField] TotalPanelUpkeepCost upkeepCost;
    [SerializeField] Button returnVillageBtn;

    [SerializeField] AudioClip enableSound;
    [SerializeField] AudioClip disableSound;
    [SerializeField] AudioClip sceneChangeSound;

    [HideInInspector] public bool isChangeScene = true;

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            ReturnVillageBtnClick();
    }

    public void SetQuantity()
    {
        returnVillageBtn.interactable = isChangeScene;

        PlayerDig.Instance.isPause = true;
        PlayerDig.Instance.rb.bodyType = RigidbodyType2D.Static;

        int[] totalPieceCnt = SaveManager.saveData.orePiece.pieceCnt;
        MineBag[] mineBags = TileManager.Instance.bags;

        for (int i = 0; i < totalPieceCnt.Length; i++)
        {
            if (totalPieceCnt[i] == 0 && mineBags[i].quantity == 0)
            {
                orePieceSlots[i].SetActive(false);
                continue;
            }

            totalQuantityTxts[i].SetText($"{totalPieceCnt[i]}<color=green>+{mineBags[i].quantity}</color>");
            orePieceSlots[i].SetActive(true);
        }

        upkeepCost.Init();
    }

    public void ReturnVillageBtnClick()
    {
        if (!isChangeScene) return;

        AudioManager.Instance.PlaySfx(sceneChangeSound);

        SaveBagOrePiece();

        SaveManager.Save();
        SceneManager.LoadScene("2.Village");
    }

    public void SaveBagOrePiece()
    {
        var totalPieceCnts = SaveManager.saveData.orePiece;
        MineBag[] mineBags = TileManager.Instance.bags;

        for (int i = 0; i < totalPieceCnts.pieceCnt.Length; i++)
        {
            totalPieceCnts.pieceCnt[i] += mineBags[i].quantity;
            SaveManager.saveData.endingLog.orePieces[i] += mineBags[i].quantity;
        }
    }

    private void OnEnable()
    {
        AudioManager.Instance.PlaySfx(enableSound);

        SetQuantity();
    }

    private void OnDisable()
    {
        AudioManager.Instance.PlaySfx(disableSound);

        if (PlayerDig.Instance != null)
        {
            PlayerDig.Instance.rb.bodyType = RigidbodyType2D.Dynamic;
            PlayerDig.Instance.isPause = false;
        }
        gameObject.SetActive(false);
    }
}
