using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleSystemRenderer))]
public class ParticleToOre : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    ParticleSystemRenderer psRenderer;

    [SerializeField] float convertDelay = 1f;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();
    }

    public void Init(Vector3 pos, float rotation, OreData oreData, int quantity, bool isCollision)
    {
        transform.position = pos;
        transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

        if (!isCollision)
            psRenderer.material = oreData.effectMaterial;
        else
        {
            psRenderer.material = oreData.pieceMaterial;
            TileManager.Instance.bags[oreData.bagIdx].quantity += quantity;

            if (TotalPanel.Instance != null && TotalPanel.Instance.gameObject.activeSelf)
                TotalPanel.Instance.SetQuantity();
        }

        var emission = ps.emission;
        var burst = emission.GetBurst(0);
        burst.count = quantity;
        emission.SetBurst(0, burst);

        var collision = ps.collision;
        collision.enabled = isCollision;

        var main = ps.main;
        var startSpeed = main.startSpeed;
        startSpeed.constantMin = isCollision ? 1f : 7f;
        startSpeed.constantMax = isCollision ? 2f : 15f;
        main.startSpeed = startSpeed;
        
        ParticleSystem.MinMaxCurve startLifeTime;
        if (!isCollision)
            startLifeTime = new(convertDelay * 2f);
        else
            startLifeTime = new(convertDelay + 0.1f, convertDelay + 0.5f);
        main.startLifetime = startLifeTime;

        var subEmitters = ps.subEmitters;
        if (!isCollision)
        {
            subEmitters.enabled = true;
            subEmitters.RemoveSubEmitter(0);
            subEmitters.AddSubEmitter(
                transform.GetChild(oreData.bagIdx).GetComponent<ParticleSystem>(),
                ParticleSystemSubEmitterType.Birth,
                ParticleSystemSubEmitterProperties.InheritNothing, 0.5f);
        }
        else
        {
            subEmitters.enabled = false;
        }

        ps.Play();

        if (isCollision)
            StartCoroutine(ConvertRoutine(oreData));

        StartCoroutine(OnStopParticle());
    }

    IEnumerator ConvertRoutine(OreData oreData)
    {
        yield return new WaitForSeconds(convertDelay);

        int count = ps.particleCount;

        if (count == 0) yield break;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            var p = particles[i];

            Vector3 worldPos = p.position;
            float size = p.GetCurrentSize(ps);
            float rotation = -p.rotation;

            particles[i] = p;

            StartCoroutine(SpawnOre(worldPos, size, rotation, oreData, p.remainingLifetime));
        }
    }

    IEnumerator SpawnOre(Vector3 pos, float size, float rot, OreData oreData, float remainingTime)
    {
        yield return new WaitForSeconds(remainingTime);

        var obj = OrePiecePool.Instance.Get();
        obj.GetComponent<OrePiece>().Init(pos, size, rot, oreData);
    }

    readonly WaitForSeconds wait = new(3f);
    IEnumerator OnStopParticle()
    {
        yield return wait;

        gameObject.SetActive(false);
        OreParticlePool.Instance.Return(gameObject);
    }
}