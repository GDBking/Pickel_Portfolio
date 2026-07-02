using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrePiece : MonoBehaviour
{
    float magnetSpeed;
    [SerializeField] float speed = 20f;

    [SerializeField] AudioClip bagInSound;
 
    SpriteRenderer spriteRenderer;

    MineBag[] bags;
    int bagIdx;

    OrePiecePool pool;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        bags = TileManager.Instance.bags;
    }

    public void Init(Vector3 pos, float size, float rotation, OreData oreData)
    {
        magnetSpeed = 0f;
        transform.position = pos;
        transform.localScale = size * Vector3.one;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);

        spriteRenderer.sprite = oreData.orePieceSprite;
        bagIdx = oreData.bagIdx;

        pool = OrePiecePool.Instance;
    }

    void Update()
    {
        magnetSpeed += speed * Time.deltaTime;

        Vector3 targetPos = GameManager.Instance.cam.ScreenToWorldPoint(bags[bagIdx].transform.position);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            magnetSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPos) < 0.2f)
        {
            Collect();
        }
    }

    public static float lastBagSoundTime;
    void Collect()
    {
        if (Time.time - lastBagSoundTime > 0.05f)
        {
            lastBagSoundTime = Time.time;
            AudioManager.Instance.PlaySfx(bagInSound);
        }

        bags[bagIdx].GetPiece();

        pool.Return(gameObject);
    }
}