using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MineBag : MonoBehaviour
{
    private static readonly int GetHash = Animator.StringToHash("Get");
    Animator anim;
    TextMeshProUGUI pieceCntTxt;
    [HideInInspector] public int quantity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pieceCntTxt = GetComponentInChildren<TextMeshProUGUI>();

        gameObject.SetActive(false);
    }

    public void GetPiece()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        anim.SetTrigger(GetHash);

        int cnt = int.Parse(pieceCntTxt.text) + 1;
        pieceCntTxt.SetText(cnt.ToString());
    }
}
