using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [HideInInspector] public PlayableDirector director;
    [HideInInspector] public NPC npc;
    [HideInInspector] public DialogueData data;

    private void Awake()
    {
        Instance = this;

        director = GetComponent<PlayableDirector>();
    }

    public void Play()
    {
        EnterCutscene();
        director.Play();
    }

    public void EnterCutscene()
    {
        PlayerDig.Instance.enabled = false;
        TotalPanel.Instance.isChangeScene = false;
    }

    public void ExitCutscene()
    {
        if (PlayerDig.Instance != null)
            PlayerDig.Instance.enabled = true;

        if (npc != null)
            npc.FadeOut();

        if (TotalPanel.Instance != null)
            TotalPanel.Instance.isChangeScene = true;
    }
}