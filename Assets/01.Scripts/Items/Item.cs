using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Item : MonoBehaviour
{
    [HideInInspector] public Vector3Int cellPos;

    protected Tilemap tilemap;
    protected PlayerDig playerDig;
    SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        tilemap = TileManager.Instance.tilemap;
        playerDig = PlayerDig.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void UseItem(Vector2 hitNormal)
    {
        spriteRenderer.enabled = false;
    }
}
