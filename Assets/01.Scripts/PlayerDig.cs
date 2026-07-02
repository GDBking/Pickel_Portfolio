using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDig : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1_5 = new(1.5f);
    private static readonly int IsDigHash = Animator.StringToHash("isDig");
    private static readonly int IsUpHash = Animator.StringToHash("isUp");
    private static readonly int IsMoveHash = Animator.StringToHash("isMove");
    public static PlayerDig Instance;

    [Header("Tilemap")]
    [SerializeField] Tilemap groundTilemap;

    [Header("Tile Data")]
    readonly Dictionary<TileBase, TileData> tileDataMap = new();
    readonly Dictionary<Vector3Int, TileState> tileStateMap = new();
    [SerializeField] TileBase notBreakTile, wallTile, entranceTile, defaultTile, grassTile;

    [Header("Move")]
    [HideInInspector] public Rigidbody2D rb;
    public float moveSpeed = 5f;
    [SerializeField] PhysicsMaterial2D groundMaterial;
    [SerializeField] PhysicsMaterial2D airMaterial;

    [Header("Up Movement")]
    public float upAcceleration = 20f;   // 상승 가속도
    public float maxUpSpeed = 5f;        // 최대 상승 속도
    [SerializeField] float gravityScaleUp = 0.5f;  // 올라갈 때 중력 약하게
    public float gravityScaleDown = 2f;  // 떨어질 때 중력 강하게

    [Header("Raycast")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float rayDistance;
    Vector2 hitNormal;
    Vector3Int cellPos;

    [Header("Pickax")]
    [HideInInspector] public int digPower;

    [Header("Playable")]
    [SerializeField] PlayableAsset entranceAsset;

    [Header("Sounds")]
    [SerializeField] AudioClip[] digSounds;
    [SerializeField] AudioClip getOreSound;
    [SerializeField] AudioClip footSteps;
    [SerializeField] AudioClip upSound;

    [HideInInspector] public Animator animator;
    PlayerInput playerInput;
    InputAction moveAction;

    [HideInInspector] public Vector2 moveInput;
    bool isFacingRight = true;
    bool isDig, isUp;
    [HideInInspector] public bool isPause;

    void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "3.Mine")
        {
            foreach (var data in TileManager.Instance.tileDatas)
            {
                tileDataMap[data.tileData.baseTile] = data.tileData;
            }
        }
    }

    void Update()
    {
        if (isDig || isPause) return;

        ApplyFrictionByState();

        if (moveInput == Vector2.zero)
        {
            animator.SetBool(IsMoveHash, false);
            return;
        }

        Vector2 dir = moveInput.normalized;
        HandleFlipX(dir);

        // 앞 타일 검사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, rayDistance, groundLayer);

        if (hit)
        {
            hitNormal = hit.normal;
            Vector2 hitPos = hit.point - hitNormal * 0.01f;
            cellPos = groundTilemap.WorldToCell(hitPos);

            // 캘 수 있으면 이동 금지 + Dig
            if (IsGrounded() && !isUp && Pickax.Instance.CurDurability > 0)
            {
                HandleDig();
                return;
            }
        }

        // 아니면 이동
        HandleMove(dir);
    }

    void FixedUpdate()
    {
        if (isDig || isPause) return;

        HandleVertical();
    }

    // ======================
    // 이동
    // ======================
    void HandleMove(Vector2 dir)
    {
        StartCoroutine(FootStepsSound());

        animator.SetBool(IsMoveHash, true);

        rb.linearVelocityX = dir.x * moveSpeed;
    }

    readonly WaitForSeconds wait = new(0.5f);
    bool isFootSteps;
    IEnumerator FootStepsSound()
    {
        if (isFootSteps || !IsGrounded()) yield break;

        isFootSteps = true;
        AudioManager.Instance.PlaySfx(footSteps);
        yield return wait;
        isFootSteps = false;
    }

    void HandleFlipX(Vector2 dir)
    {
        // 방향 처리
        if (dir.x > 0 && !isFacingRight)
        {
            Flip(true);
        }
        else if (dir.x < 0 && isFacingRight)
        {
            Flip(false);
        }
    }

    void HandleVertical()
    {
        isUp = moveInput.y > 0.1f;

        // 애니메이션
        animator.SetBool(IsUpHash, isUp);

        float vy = rb.linearVelocityY;

        if (isUp)
        {
            StartCoroutine(UpSound());

            // 위로 가속
            vy += upAcceleration * Time.fixedDeltaTime;
            vy = Mathf.Min(vy, maxUpSpeed);

            rb.gravityScale = gravityScaleUp;
        }
        else
        {
            // 버튼 떼면 중력 강화
            rb.gravityScale = gravityScaleDown;
        }

        rb.linearVelocityY = vy;
    }

    bool isUpSound;
    IEnumerator UpSound()
    {
        if (isUpSound) yield break;

        isUpSound = true;
        AudioManager.Instance.PlaySfx(upSound);
        yield return _waitForSeconds1_5;
        isUpSound = false;
    }

    public void Flip(bool faceRight)
    {
        isFacingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = faceRight ? 1f : -1f;
        transform.localScale = scale;
    }

    // ======================
    // 채굴
    // ======================
    void HandleDig()
    {
        isDig = true;

        rb.linearVelocityX = 0f;

        animator.SetTrigger(IsDigHash);
    }

    // ======================
    // 애니메이션 이벤트에서 호출
    // ======================
    void DigAnim()
    {
        TryDig(cellPos, hitNormal, digPower, false);
        isDig = false;

        SaveManager.saveData.endingLog.pickaxCnt++;
    }

    public bool TryDig(Vector3Int cellPos, Vector2 hitNormal, int digPower, bool isBomb)
    {
        TileBase tile = groundTilemap.GetTile(cellPos);

        if (tile == null) return false;
        if (tile == notBreakTile || tile == wallTile)
        {
            if (!isBomb)
                AudioManager.Instance.PlaySfx(digSounds[2]);
            
            return false;
        }
        if (tile == entranceTile)
        {
            if (isBomb)
                return false;

            CutsceneManager.Instance.director.playableAsset = entranceAsset;
            CutsceneManager.Instance.director.Play();

            return false;
        }


        if (!tileStateMap.TryGetValue(cellPos, out var state))
        {
            TileData tileData = tileDataMap[tile];

            state = new TileState
            {
                curHP = tileData.maxHP,
                data = tileData
            };

            tileStateMap[cellPos] = state;
        }

        TileBase tile2 = state.data.baseTile;
        if (!isBomb)
        {
            if (tile2 == defaultTile || tile2 == grassTile)
                AudioManager.Instance.PlaySfx(digSounds[0]);
            else
                AudioManager.Instance.PlaySfx(digSounds[1]);
        }

        state.curHP -= digPower;
        if (!isBomb)
            Pickax.Instance.SetDurability(-1);

        // 파괴
        if (state.curHP <= 0)
        {
            SpawnDigParticle(cellPos);

            groundTilemap.SetTile(cellPos, null);
            tileStateMap.Remove(cellPos);

            if (TileManager.Instance.posInOre.TryGetValue(cellPos, out GameObject oreObj))
            {
                AudioManager.Instance.PlaySfx(getOreSound);
                StartCoroutine(OreParticle(cellPos, oreObj.GetComponent<OreObj>(), hitNormal));
            }
            else if (TileManager.Instance.posInItems.TryGetValue(cellPos, out GameObject itemObj))
                itemObj.GetComponent<Item>().UseItem(hitNormal);

            return true;
        }

        // 체력 비율 (0 ~ 1)
        float hpRatio = (float)state.curHP / state.data.maxHP;
        // stage 계산 (0 ~ maxStage-1)
        int stage = 3;
        switch (hpRatio)
        {
            case <= 0.25f:
                stage = 2;
                break;
            case <= 0.5f:
                stage = 1;
                break;
            case <= 0.75f:
                stage = 0;
                break;
        }

        if (stage != 3)
            groundTilemap.SetTile(cellPos, state.data.damageTiles[stage]);

        return true;
    }

    readonly WaitForFixedUpdate fixedWait = new();
    [HideInInspector] public bool isOrePotion;
    [HideInInspector] public bool isMaxOrePotion;
    IEnumerator OreParticle(Vector3Int cellPos, OreObj obj, Vector2 hitNormal)
    {
        yield return fixedWait;

        Vector3 spawnPos = groundTilemap.GetCellCenterWorld(cellPos);
        float rotation = hitNormal switch
        {
            var n when n.x < -0.5f => 90f,
            var n when n.x > 0.5f => -90f,
            var n when n.y > 0.5f => 0f,
            _ => 180f
        };

        int cnt = Mathf.RoundToInt(Random.Range(TileManager.Instance.orePieceMin, TileManager.Instance.orePieceMax * (isMaxOrePotion ? 2 : 1) + 1) * (isOrePotion ? 1.5f : 1f));
        GameObject go = OreParticlePool.Instance.Get();
        go.GetComponent<ParticleToOre>().Init(spawnPos, rotation, obj.oreData, cnt, true);
        GameObject go2 = OreParticlePool.Instance.Get();
        go2.GetComponent<ParticleToOre>().Init(spawnPos, rotation, obj.oreData, cnt, false);

        Destroy(obj.gameObject);
    }

    void SpawnDigParticle(Vector3Int cellPos)
    {
        GameObject go = Dig_ParticlePool.Instance.Get();
        go.transform.position = groundTilemap.GetCellCenterWorld(cellPos);
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
    }

    void ApplyFrictionByState()
    {
        if (IsGrounded())
        {
            if (rb.sharedMaterial != groundMaterial)
                rb.sharedMaterial = groundMaterial;
        }
        else
        {
            if (rb.sharedMaterial != airMaterial)
                rb.sharedMaterial = airMaterial;
        }
    }

    void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCancel;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCancel;

        moveInput = Vector2.zero;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCancel(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    public void PowerPotion(float duration)
    {
        powerDuration = duration;
        StartCoroutine(PowerPotionRoutine());
    }

    float powerDuration;
    [HideInInspector] public bool isPowerUp;
    float powerElapsedTime;
    IEnumerator PowerPotionRoutine()
    {
        if (isPowerUp)
        {
            powerElapsedTime = 0f;
            yield break;
        }

        isPowerUp = true;

        animator.speed = 2f;
        moveSpeed *= 1.5f;
        digPower += 999;
        upAcceleration *= 1.5f;
        maxUpSpeed *= 1.5f;
        gravityScaleDown *= 1.5f;

        while (powerElapsedTime < powerDuration)
        {
            yield return null;
            powerElapsedTime += Time.deltaTime;
        }
        powerElapsedTime = 0f;

        animator.speed = 1f;
        moveSpeed /= 1.5f;
        digPower -= 999;
        upAcceleration /= 1.5f;
        maxUpSpeed /= 1.5f;
        gravityScaleDown /= 1.5f;

        isPowerUp = false;
    }


    public void OrePotion(float duration, bool isMaxOrePotion)
    {
        oreDuration = duration;
        StartCoroutine(OrePotionRoutine(isMaxOrePotion));
    }

    float oreDuration;
    float elapsedTime;
    float maxElapsedTime;
    IEnumerator OrePotionRoutine(bool isMaxOrePotion)
    {
        if (isMaxOrePotion)
        {
            if (this.isMaxOrePotion)
            {
                maxElapsedTime = 0f;
                yield break;
            }
            this.isMaxOrePotion = true;

            while (maxElapsedTime < oreDuration)
            {
                yield return null;
                maxElapsedTime += Time.deltaTime;
            }
            this.isMaxOrePotion = false;
        }
        else
        {
            if (isOrePotion)
            {
                elapsedTime = 0f;
                yield break;
            }
            isOrePotion = true;

            while (elapsedTime < oreDuration)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }
            isOrePotion = false;
        }
    }
}

// ======================
// Tile 데이터
// ======================

public class TileState
{
    public int curHP;
    public TileData data;
}