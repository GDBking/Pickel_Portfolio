using UnityEngine;

public class DigParticle : MonoBehaviour
{
    private void OnDisable()
    {
        Dig_ParticlePool.Instance.Return(gameObject);
    }
}
