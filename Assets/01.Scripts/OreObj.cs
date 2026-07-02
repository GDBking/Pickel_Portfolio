using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OreObj : MonoBehaviour
{
    [SerializeField] GameObject twinkleParticle;

    SpriteRenderer spriteRenderer;
    [HideInInspector] public OreData oreData;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (SaveManager.saveData.village.mog.twinkleProb >= Random.Range(1, 101))
            twinkleParticle.SetActive(true);
    }

    public void Init(OreData oreData)
    {
        this.oreData = oreData;
        spriteRenderer.sprite = oreData.oreSprite;
    }
}
