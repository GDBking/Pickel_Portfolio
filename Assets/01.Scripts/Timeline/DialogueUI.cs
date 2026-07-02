using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [SerializeField] TMP_Text dialogueText;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(string text)
    {
        gameObject.SetActive(true);
        dialogueText.text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTextPos(string key, Color textColor)
    {
        Vector2 pos;
        if (key != "p")
        {
            if (TileManager.Instance != null)
                pos = GameManager.Instance.cam.WorldToScreenPoint((Vector2)PlayerDig.Instance.transform.position + Vector2.up + Vector2.left * TileManager.Instance.tilemap.transform.localScale.x);
            else
                pos = GameManager.Instance.cam.WorldToScreenPoint((Vector2)PlayerDig.Instance.transform.position + Vector2.up + (CutsceneManager2.Instance.curIsFlipX ? Vector2.right : Vector2.left) * 1.2f);
            dialogueText.color = textColor;
        }
        else
        {
            pos = GameManager.Instance.cam.WorldToScreenPoint((Vector2)PlayerDig.Instance.transform.position + Vector2.up);
            dialogueText.color = Color.white;
        }

        dialogueText.GetComponent<RectTransform>().position = pos;
    }

    public void SetTextPos2(Vector2 pos, string key, Color textColor)
    {
        pos = GameManager.Instance.cam.WorldToScreenPoint(pos + Vector2.up * 1.5f);
        dialogueText.color = key == "p" ? Color.white : textColor;
        dialogueText.GetComponent<RectTransform>().position = pos;
    }
}