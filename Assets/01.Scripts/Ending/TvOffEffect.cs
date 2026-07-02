using System.Collections;
using UnityEngine;

public class TvOffEffect : MonoBehaviour
{
    [Header("Black Panels")]
    [SerializeField] RectTransform topBlack;
    [SerializeField] RectTransform bottomBlack;

    [Header("White Line")]
    [SerializeField] RectTransform whiteLine;

    [Header("Settings")]
    [SerializeField] float closeDuration = 0.15f;
    [SerializeField] float lineDuration = 0.08f;
    [SerializeField] float lineShrinkDuration = 0.08f;

    Canvas rootCanvas;
    RectTransform canvasRect;

    bool isPlaying;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다.");
            return;
        }

        canvasRect = rootCanvas.GetComponent<RectTransform>();

        ResetEffect();
    }

    public void Play()
    {
        if (isPlaying)
            return;

        ResetEffect();

        StartCoroutine(CoPlay());
    }

    IEnumerator CoPlay()
    {
        isPlaying = true;

        float halfHeight = canvasRect.rect.height * 0.5f;
        float canvasWidth = canvasRect.rect.width;

        //--------------------------------
        // 1. 위아래 검은 화면 닫기
        //--------------------------------

        float time = 0f;

        while (time < closeDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / closeDuration);

            float height = Mathf.Lerp(0f, halfHeight, t);

            topBlack.sizeDelta =
                new Vector2(topBlack.sizeDelta.x, height);

            bottomBlack.sizeDelta =
                new Vector2(bottomBlack.sizeDelta.x, height);

            yield return null;
        }

        //--------------------------------
        // 2. 흰 줄 표시
        //--------------------------------

        whiteLine.gameObject.SetActive(true);

        yield return new WaitForSeconds(lineDuration);

        //--------------------------------
        // 3. 흰 줄 가로축 수축
        //--------------------------------

        time = 0f;

        while (time < lineShrinkDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / lineShrinkDuration);

            float width = Mathf.Lerp(canvasWidth, 0f, t);

            whiteLine.sizeDelta =
                new Vector2(width, whiteLine.sizeDelta.y);

            yield return null;
        }

        whiteLine.gameObject.SetActive(false);

        isPlaying = false;
    }

    void ResetEffect()
    {
        if (canvasRect == null)
            return;

        float canvasWidth = canvasRect.rect.width;

        topBlack.sizeDelta =
            new Vector2(topBlack.sizeDelta.x, 0f);

        bottomBlack.sizeDelta =
            new Vector2(bottomBlack.sizeDelta.x, 0f);

        whiteLine.sizeDelta =
            new Vector2(canvasWidth, 3f);

        whiteLine.gameObject.SetActive(false);
    }
}