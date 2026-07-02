using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDownMove : MonoBehaviour
{
    [SerializeField] LayerMask platformLayer;
    Collider2D playerCollider;
    bool isIgnore;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (PlayerDig.Instance.moveInput.y < -0.5f && !isIgnore) 
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, platformLayer);
            if (hit.collider != null)
            {
                StartCoroutine(IgnoreCollisionRoutine(hit));
            }
        }
    }

    readonly WaitForSeconds wait = new(0.5f);
    IEnumerator IgnoreCollisionRoutine(RaycastHit2D hit)
    {
        isIgnore = true;
        Physics2D.IgnoreCollision(playerCollider, hit.collider, true);
        yield return wait;
        Physics2D.IgnoreCollision(playerCollider, hit.collider, false);
        isIgnore = false;
    }
}
