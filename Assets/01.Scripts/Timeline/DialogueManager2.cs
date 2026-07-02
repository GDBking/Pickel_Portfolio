using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager2 : MonoBehaviour
{
    [SerializeField] AudioClip dialogueSound;
    [SerializeField] GameObject mouseClick;

    const string END_MARK = "**";

    public void StartDialogue()
    {
        StartCoroutine(ShowDialogueSequence(CutsceneManager2.Instance.data));
    }

    public IEnumerator ShowDialogueSequence(DialogueData data)
    {
        CutsceneManager2.Instance.director.Pause();

        int row = data.minRow;

        while (true)
        {
            DialogueParser text = LocalizationManager.Instance.Get(row);

            if (text.contents.StartsWith("**"))
                break;

            yield return null;

            DialogueUI.Instance.SetTextPos(text.talker, data.textColor);

            yield return StartCoroutine(TypeText(text.contents));

            yield return new WaitUntil(() => Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame);

            DialogueUI.Instance.Show("");

            row++;
        }

        if (data.minRow == 24)
        {
            mouseClick.SetActive(true);
        }

        DialogueUI.Instance.Hide();

        CutsceneManager2.Instance.director.Play();
    }

    readonly WaitForSeconds wait = new(0.03f);
    IEnumerator TypeText(string text)
    {
        DialogueUI.Instance.Show("");

        for (int i = 0; i < text.Length; i++)
        {
            if (i % 5 == 0)
                AudioManager.Instance.PlaySfx(dialogueSound);

            DialogueUI.Instance.Show(text[..(i + 1)]);

            yield return wait;
        }
    }
}