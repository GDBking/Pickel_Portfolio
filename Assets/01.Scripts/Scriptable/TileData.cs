using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public TileBase baseTile;
    public TileBase[] damageTiles;

    public int maxHP;

    public OreData oreData;
}

[Serializable]
public class OreData
{
    public Sprite oreSprite;
    public Sprite orePieceSprite;
    public Material effectMaterial;
    public Material pieceMaterial;
    public int bagIdx;
}
