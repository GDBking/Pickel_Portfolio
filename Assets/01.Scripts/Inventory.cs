using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Inventory : MonoBehaviour
{
    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Update()
    {
        if ((Keyboard.current.iKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame) && btn.interactable)
            BtnClick();
    }

    public void BtnClick()
    {
        TotalPanel.Instance.gameObject.SetActive(!TotalPanel.Instance.gameObject.activeSelf);
    }
}
