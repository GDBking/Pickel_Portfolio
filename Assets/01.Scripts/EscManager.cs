using UnityEngine;
using UnityEngine.InputSystem;

public class EscManager : MonoBehaviour
{
    [Header("빌드 패널")]
    [SerializeField] private GameObject[] buildPanels;

    [SerializeField] private GameObject option;
    [SerializeField] private GameObject esc;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EscOnOff();
        }
    }

    private void EscOnOff()
    {
        if (buildPanels != null)
        {
            foreach (GameObject panel in buildPanels)
            {
                if (panel != null && panel.activeSelf)
                {
                    panel.SetActive(false);
                    return;
                }
            }
        }

        if (option != null && option.activeSelf)
        {
            option.SetActive(false);
            return;
        }

        if (esc != null)
        {
            esc.SetActive(!esc.activeSelf);
        }
    }
}