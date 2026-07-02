using UnityEngine;

public class LobbyParallax : MonoBehaviour
{
    [Header("스크롤 설정")]
    [Tooltip("배경이 이동하는 속도입니다.")]
    [Range(0f, 1f)]
    public float scrollSpeed = 0.5f;

    [Tooltip("배경이 왼쪽으로 흐르게 하려면 True, 오른쪽은 False")]
    public bool scrollLeft = true;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        // 초기 높이와 Z축 값을 고정합니다.
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void Update()
    {
        // 방향 결정 (왼쪽: -1, 오른쪽: 1)
        float direction = scrollLeft ? -1f : 1f;

        // 매 프레임마다 속도와 시간에 따라 위치를 이동시킵니다.
        float moveAmount = direction * scrollSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveAmount, 0, 0));

        // Y축과 Z축이 어긋나지 않도록 다시 한번 고정 (필요 시)
        transform.position = new Vector3(transform.position.x, fixedY, fixedZ);
    }
}