using UnityEngine;
using UnityEngine.EventSystems;

public class BtnSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioClip enterSound;
    [SerializeField] AudioClip clickSound;

    AudioManager audioMng;

    private void Start()
    {
        audioMng = AudioManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enterSound != null)
            audioMng.PlaySfx(enterSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            audioMng.PlaySfx(clickSound);
    }
}
