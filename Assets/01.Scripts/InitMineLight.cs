using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InitMineLight : MonoBehaviour
{
    [SerializeField] Light2D light2D;

    private void Awake()
    {
        StartCoroutine(LightOn());
    }

    IEnumerator LightOn()
    {
        while (light2D.intensity < 2f)
        {
            yield return null;
            light2D.intensity += Time.deltaTime;
        }

        light2D.intensity = 1f;
    }
}
