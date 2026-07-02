using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteOutline))]
public class BuildingInteract : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject uiPanel;
    SpriteOutline outline;

    private void Awake()
    {
        outline = GetComponent<SpriteOutline>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        outline.enabled = false;
        uiPanel.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}
