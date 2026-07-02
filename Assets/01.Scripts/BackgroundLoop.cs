using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public Transform cameraTransform;

    private float spriteWidth;
    private Transform[] backgrounds;
    private int backgroundCount;

    void Start()
    {
        backgroundCount = transform.childCount;
        backgrounds = new Transform[backgroundCount];

        // 1. 첫 번째 자식의 SpriteRenderer를 이용해 실제 너비를 자동 계산합니다.
        SpriteRenderer spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            Debug.LogError("자식 오브젝트에 SpriteRenderer가 없습니다!");
            return;
        }

        // 2. 모든 자식들을 backgrounds 배열에 넣고, 빈틈없이 일렬로 재배치합니다.
        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);

            // 첫 번째 배경의 위치를 기준으로 spriteWidth만큼 따닥따닥 붙입니다.
            float newX = backgrounds[0].position.x + (i * spriteWidth);
            backgrounds[i].position = new Vector3(newX, backgrounds[i].position.y, backgrounds[i].position.z);
        }
    }

    void Update()
    {
        // 전체 배경 타일들이 차지하는 총 너비
        float totalWidth = spriteWidth * backgroundCount;

        foreach (Transform bg in backgrounds)
        {
            // 카메라와 각 배경 타일 중심 사이의 거리 차이
            float distance = cameraTransform.position.x - bg.position.x;

            // 카메라가 한 타일의 절반 이상을 지나치면 반대편 끝으로 이동
            if (distance > totalWidth / 2f)
            {
                bg.position += new Vector3(totalWidth, 0, 0);
            }
            else if (distance < -totalWidth / 2f)
            {
                bg.position -= new Vector3(totalWidth, 0, 0);
            }
        }
    }
}