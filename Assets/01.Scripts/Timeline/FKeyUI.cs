using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FKeyUI : MonoBehaviour
{
    private static readonly int IsActiveHash = Animator.StringToHash("IsActive");
    public static FKeyUI Instance;

    Animator anim;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        anim = GetComponent<Animator>();
    }

    public void FKeyOn()
    {
        if (!anim.GetBool(IsActiveHash))
        {
            anim.SetBool(IsActiveHash, true);
            gameObject.SetActive(true);
        }
    }

    public void FKeyOff()
    {
        if (anim.GetBool(IsActiveHash))
            anim.SetBool(IsActiveHash, false);
    }

    void FKeyDisable()
    {
        gameObject.SetActive(false);
    }
}
