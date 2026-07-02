using UnityEngine;

[CreateAssetMenu(fileName = "LunaData", menuName = "Scriptable Objects/LunaData")]
public class LunaData : ScriptableObject
{
    public int min, max;
    public OrePieceData piece;
}
