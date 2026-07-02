using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueIdx
{
    public int idx;
    public DialogueData dialogueData;
    public Transform npcTransform;
    public Sprite objSpr;
}

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager2 : MonoBehaviour
{
    public static CutsceneManager2 Instance;

    [HideInInspector] public PlayableDirector director;
    [HideInInspector] public DialogueData data;
    [SerializeField] List<DialogueIdx> dialogueIdxes;
    [HideInInspector] public bool curIsFlipX;
    [SerializeField] SpriteRenderer objectImg;

    private void Awake()
    {
        Instance = this;

        director = GetComponent<PlayableDirector>();
        director.stopped += ExitCutscene;
    }

    private void Start()
    {
        if (SaveManager.saveData.npc.isDialogue)
        {
            int unlockIdx = SaveManager.saveData.npc.unlockIdx;
            foreach (var dialogue in dialogueIdxes)
            {
                if (dialogue.idx == unlockIdx)
                {
                    curIsFlipX = dialogue.npcTransform.localScale.x < 0f;

                    PlayerDig.Instance.transform.position = dialogue.npcTransform.position + (curIsFlipX ? Vector3.left : Vector3.right) * 1.2f;
                    PlayerDig.Instance.Flip(curIsFlipX);

                    if (dialogue.objSpr != null)
                    {
                        objectImg.sprite = dialogue.objSpr;
                        objectImg.gameObject.SetActive(true);
                    }

                    data = dialogue.dialogueData;
                    Play();
                    break;
                }
            }
        }
    }

    public void Play()
    {
        EnterCutscene();
        director.Play();
    }

    public void EnterCutscene()
    {
        PlayerDig.Instance.enabled = false;
        VillageManager.Instance.isChangeScene = false;
    }

    public void ExitCutscene(PlayableDirector d)
    {
        if (PlayerDig.Instance != null)
            PlayerDig.Instance.enabled = true;

        if (objectImg != null)
            objectImg.gameObject.SetActive(false);

        SaveManager.saveData.npc.isDialogue = false;

        if (VillageManager.Instance != null)
            VillageManager.Instance.isChangeScene = true;

        SaveManager.Save();
    }

    public void InitEnd()
    {
        SaveManager.saveData.isInit = true;
    }
}