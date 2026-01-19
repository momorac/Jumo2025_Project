using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlacementRecord
{
    public bool occupied;
    public GameObject prefab;
    public Vector2Int root;
    public Transform transform;
}