using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] float noiseDuration;
    [SerializeField] AudioClip lobbyBgm;
    [SerializeField] GameObject noise;
    [SerializeField] AudioClip noiseSound;

    [SerializeField] GameObject continueBtn;
    [SerializeField] GameObject gameStartPanel;

    private void Start()
    {
        bool isDataExist = SaveManager.Load();
        continueBtn.SetActive(isDataExist);

        AudioManager.Instance.PlayBgm(lobbyBgm);

        StartCoroutine(NoiseEffect());
    }

    public void StartBtnClick()
    {
        if (SaveManager.saveData.isInit)
        {
            gameStartPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("3.Mine");
        }
    }

    public void StartPanelClick()
    {
        SaveManager.DeleteSave();
        SceneManager.LoadScene("3.Mine");
    }

    public void ContinueBtnClick()
    {
        if (SaveManager.saveData.isInit)
            SceneManager.LoadScene("2.Village");
        else
        {
            SceneManager.LoadScene("3.Mine");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    float elapsedTime;
    IEnumerator NoiseEffect()
    {
        AudioManager.Instance.PlaySfx(noiseSound, 0.1f);

        while (elapsedTime < noiseDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        noise.SetActive(false);
    }
}