using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;

    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private float previousCamX;
    private float fixedY;

    void Start()
    {
        previousCamX = cameraTransform.position.x;
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        float deltaX = cameraTransform.position.x - previousCamX;

        transform.position += new Vector3(deltaX * parallaxFactor, 0f, 0f);
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        previousCamX = cameraTransform.position.x;
    }
}