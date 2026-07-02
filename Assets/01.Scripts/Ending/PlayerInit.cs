using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayerInit : MonoBehaviour
{
    [SerializeField] PlayableAsset playableAsset;
    [SerializeField] DialogueData[] datas;

    private void Start()
    {
        if (SaveManager.saveData.isInit)
            return;

        CutsceneManager.Instance.director.playableAsset = playableAsset;
        CutsceneManager.Instance.data = datas[0];
        CutsceneManager.Instance.Play();
    }

    public void ChangeDialogue()
    {
        CutsceneManager.Instance.data = datas[1];
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("2.Village");
    }
}
