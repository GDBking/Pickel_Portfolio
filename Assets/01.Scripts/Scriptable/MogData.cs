using UnityEngine;

[CreateAssetMenu(fileName = "MogData", menuName = "Scriptable Objects/MogData")]
public class MogData : ScriptableObject
{
    public float inner;
    public float outer;
    public int twinkleProb;
    public OrePieceData piece;
}
