using System.Collections.Generic;
using UnityEngine;

public class OreParticlePool : MonoBehaviour
{
    public static OreParticlePool Instance;

    public GameObject prefab;
    public int size = 30;

    readonly Queue<GameObject> pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab, transform);
    }

    public void Return(GameObject obj)
    {
        pool.Enqueue(obj);
    }
}
