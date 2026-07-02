using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour
{
    protected readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int IsMoveHash = Animator.StringToHash("isMove");
    private readonly int IsUpHash = Animator.StringToHash("isUp");
    [SerializeField] DialogueData data;
    public bool isOnTheGround;
    [SerializeField] bool isNextNPC;
    [SerializeField] bool isObject;
    protected Animator anim;

    Tilemap tilemap;
    GameObject player;
    Vector3Int cellPos;

    SpriteRenderer sr;

    bool isInteraction;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (isOnTheGround || isObject)
            anim.SetBool(IdleHash, true);
    }

    private void Start()
    {
        tilemap = TileManager.Instance.tilemap;
        player = PlayerDig.Instance.gameObject;
        cellPos = tilemap.WorldToCell(transform.position);

        sr = GetComponent<SpriteRenderer>();

        CutsceneManager.Instance.npc = this;
        CutsceneManager.Instance.data = data;
    }

    private void Update()
    {
        if (isInteraction) return;

        Vector3Int playerCellPos = tilemap.WorldToCell(player.transform.position);
        if (!isInteraction && (playerCellPos == cellPos + Vector3Int.left || playerCellPos == cellPos + Vector3Int.right))
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

    public virtual void Interact()
    {
        if (TotalPanel.Instance.gameObject.activeSelf) return;

        isInteraction = true;

        AudioManager.Instance.PlaySfx(GameManager.Instance.npcInteractionSound);

        PlayerDig.Instance.animator.SetBool(IsUpHash, false);
        PlayerDig.Instance.animator.SetBool(IsMoveHash, false);
        PlayerDig.Instance.rb.linearVelocity = Vector2.zero;
        player.transform.position = tilemap.CellToWorld(cellPos + Vector3Int.right) + Vector3.right * tilemap.transform.localScale.x / 2f;
        PlayerDig.Instance.Flip(false);

        CutsceneManager.Instance.Play();
    }

    public void FadeOut()
    {
        var saveData = SaveManager.saveData.npc;
        saveData.unlockIdx++;
        if (!isNextNPC)
            saveData.isUnlock = false;
        saveData.isDialogue = true;

        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        GetComponent<Collider2D>().enabled = false;

        float time = 0f;
        Color color = sr.color;

        while (time < 1.5f)
        {
            time += Time.deltaTime;
            float t = time / 1.5f;

            color.a = Mathf.Lerp(1f, 0f, t);
            sr.color = color;

            yield return null;
        }

        // 확실히 0으로 마무리
        color.a = 0f;
        sr.color = color;

        Destroy(gameObject);
    }
}