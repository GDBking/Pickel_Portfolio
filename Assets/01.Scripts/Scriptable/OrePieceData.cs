using System;
using UnityEngine;

public enum OreEnum
{
    Coal,
    Tin,
    Cooper,
    Iron,
    Gold,
    Sapphire,
    Ruby,
    Emerald,
    Diamond,
    RedDiamond
}

[CreateAssetMenu(fileName = "OrePieceData", menuName = "Scriptable Objects/OrePieceData")]
public class OrePieceData : ScriptableObject
{
    public OrePiece[] orePieces;

    [Serializable]
    public class OrePiece
    {
        public OreEnum type;
        public int amount;
    }
}