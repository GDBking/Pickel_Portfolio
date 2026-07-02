using UnityEngine;

public class TotalBag : MonoBehaviour
{
    public static TotalBag Instance;

    public VillageBag[] bags;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < bags.Length; i++)
        {
            bags[i].SetCount(i);
        }
    }
}
