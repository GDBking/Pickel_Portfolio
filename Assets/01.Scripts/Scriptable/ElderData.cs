using UnityEngine;

[CreateAssetMenu(fileName = "ElderData", menuName = "Scriptable Objects/ElderData")]
public class ElderData : ScriptableObject
{
    public Sprite spr;
    public int x, y;
    public int baseTileHP;
    public OreEnum oreIdx;
    public OrePieceData piece;
}
