using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            OnClick();
    }

    public void OnClick()
    {
        if (VillageManager.Instance.isChangeScene && UpkeepCost.Instance.isBuy)
        {
            SaveManager.Save();
            StartCoroutine(ChangeMiningScene());
        }
    }

    IEnumerator ChangeMiningScene()
    {
        VillageManager.Instance.blockPanel.SetActive(true);
        VillageManager.Instance.isChangeScene = false;

        UpkeepCost.Instance.gameObject.SetActive(!UpkeepCost.Instance.isNull);

        if (UpkeepCost.Instance.ChangeMiningScene())
            yield return UtilManager.completedWait;

        SaveManager.saveData.village.crow.isSetting = false;

        SaveManager.saveData.endingLog.mineCnt++;

        SceneManager.LoadScene("3.Mine");
    }
}
