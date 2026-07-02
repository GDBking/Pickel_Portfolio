using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panelToShow;

    private void OnEnable()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(false);
        }
    }
}