using UnityEngine;
using UnityEngine.EventSystems;

public class ElderClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject mouseClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mouseClick.activeSelf)
            mouseClick.SetActive(false);
    }
}
