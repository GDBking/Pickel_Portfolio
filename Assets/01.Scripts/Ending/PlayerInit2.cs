using UnityEngine;
using UnityEngine.Playables;

public class PlayerInit2 : MonoBehaviour
{
    [SerializeField] PlayableAsset playableAsset;
    [SerializeField] DialogueData data;

    private void Start()
    {
        if (SaveManager.saveData.isInit)
            return;

        CutsceneManager2.Instance.director.playableAsset = playableAsset;
        CutsceneManager2.Instance.data = data;
        CutsceneManager2.Instance.Play();
    }
}
