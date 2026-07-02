using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class PlayerLight2D : MonoBehaviour
{
    Light2D light2D;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        SetLight();
    }

    void SetLight()
    {
        var saveData = SaveManager.saveData.village.mog;

        light2D.pointLightInnerRadius = saveData.inner;
        light2D.pointLightOuterRadius = saveData.outer;
    }
}
