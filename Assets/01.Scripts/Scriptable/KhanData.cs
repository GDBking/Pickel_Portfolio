using UnityEngine;

[CreateAssetMenu(fileName = "KhanData", menuName = "Scriptable Objects/KhanData")]
public class KhanData : ScriptableObject
{
    public Sprite spr;
    public int power;
    public int durability;
    public OrePieceData piece;
}
