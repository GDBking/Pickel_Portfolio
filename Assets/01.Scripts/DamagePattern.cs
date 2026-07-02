using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DamageCell
{
    public Vector3Int offset;
    public Vector2 direction;
}

[Serializable]
public class DamagePattern
{
    public int range = 1;
    public List<DamageCell> cells = new();
    public bool isDir;
}
