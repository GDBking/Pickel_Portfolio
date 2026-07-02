using UnityEngine;

[CreateAssetMenu(fileName = "BoomData", menuName = "Scriptable Objects/BoomData")]
public class BoomData : ScriptableObject
{
    public Sprite bombSprite;
    public Sprite rangeSprite;
    public OrePieceData piece;
}
