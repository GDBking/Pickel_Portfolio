using System.Collections.Generic;
using UnityEngine;

public class OrePiecePool : MonoBehaviour
{
    public static OrePiecePool Instance;

    public GameObject prefab;
    public int size = 100;

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
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab, transform);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}