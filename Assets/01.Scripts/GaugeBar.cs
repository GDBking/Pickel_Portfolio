using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    public static GaugeBar Instance;

    [SerializeField] Transform player;
    [SerializeField] Image[] barImgs;

    Vector3 offset = new(0f, -1.2f);

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        transform.position = GameManager.Instance.cam.WorldToScreenPoint(player.position + offset);
    }

    public void SetGaugeBar(int idx, float duration)
    {
        StartCoroutine(SetGaugeBarRoutine(idx, duration));
    }

    readonly float[] elapsedTimes = new float[3];
    IEnumerator SetGaugeBarRoutine(int idx, float duration)
    {
        if (elapsedTimes[idx] > 0f)
        {
            elapsedTimes[idx] = duration;
            yield break;
        }

        barImgs[idx].fillAmount = 1f;
        barImgs[idx].transform.parent.gameObject.SetActive(true);
        elapsedTimes[idx] = duration;
        while (elapsedTimes[idx] > 0f)
        {
            yield return null;
            elapsedTimes[idx] -= Time.deltaTime;
            barImgs[idx].fillAmount = elapsedTimes[idx] / duration;
        }
        barImgs[idx].transform.parent.gameObject.SetActive(false);
    }
}
