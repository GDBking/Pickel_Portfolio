using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class ElderInteract : MonoBehaviour
{
    [SerializeField] DialogueData[] datas;
    [SerializeField] PlayableAsset playableAsset;

    private readonly int IsMoveHash = Animator.StringToHash("isMove");
    private readonly int IsUpHash = Animator.StringToHash("isUp");
    GameObject player;

    bool isInteraction;

    private void Start()
    {
        player = PlayerDig.Instance.gameObject;
    }

    private void Update()
    {
        if (isInteraction) return;

        if (!isInteraction && Vector2.Distance(transform.position, player.transform.position) <= 2f)
        {
            FKeyUI.Instance.FKeyOn();

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                FKeyUI.Instance.FKeyOff();
                Interact();
            }
        }
        else
        {
            FKeyUI.Instance.FKeyOff();
        }
    }

    void Interact()
    {
        if (TotalPanel.Instance.gameObject.activeSelf) return;

        isInteraction = true;

        PlayerDig.Instance.animator.SetBool(IsUpHash, false);
        PlayerDig.Instance.animator.SetBool(IsMoveHash, false);
        PlayerDig.Instance.rb.linearVelocity = Vector2.zero;
        player.transform.position = transform.position + Vector3.right * 2f;
        PlayerDig.Instance.Flip(false);

        CutsceneManager.Instance.data = datas[0];
        CutsceneManager.Instance.director.playableAsset = playableAsset;
        CutsceneManager.Instance.Play();
    }

    public void SetDialogue()
    {
        CutsceneManager.Instance.data = datas[1];
    }
}
