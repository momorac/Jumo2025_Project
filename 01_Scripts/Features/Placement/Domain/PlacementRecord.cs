using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlacementRecord
{
    public bool occupied;
    public PlaceableDTO placeable;
    public Int2 root;
    public Transform transform;

    public PlacementRecord()
    {
        this.occupied = false;
        this.placeable = new PlaceableDTO();
        this.root = new Int2(-1, -1);
        this.transform = null;
    }
}

public struct PlaceableDTO
{
    public PlaceableType type;
    public FacilityType facility;
    public TileType tile;
    public DecorationType decoration;
}