using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlacementRecord
{
    public bool occupied;
    public Placeable placedType;
    public Int2 root;
    public Transform transform;

    public PlacementRecord()
    {
        this.occupied = false;
        this.placedType = null;
        this.root = new Int2(-1, -1);
        this.transform = null;
    }
}