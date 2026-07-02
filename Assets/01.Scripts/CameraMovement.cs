using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    
    Vector3 target;

    private void LateUpdate()
    {
        target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 10);
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5);
    }
}