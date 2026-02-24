using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMetaData
{
    public PlaceableData PlaceableData;
    public PlacementData PlacementData;
    public Economy EconomyData;
}

[Serializable]
public class SessionState
{
    public Dictionary<Seat, bool> Seats = new Dictionary<Seat, bool>();
    public int AvailableSeatsCount;
}

[Serializable]
public class PlaceableData
{
    public HashSet<FacilityType> ul_facility = new HashSet<FacilityType>();
    public HashSet<TileType> ul_tile = new HashSet<TileType>();
    public HashSet<DecorationType> ul_decoration = new HashSet<DecorationType>();
}

[Serializable]
public class PlacementData
{
    public Int2 GridSize;
    public PlacementRecord[,] Placements;

    public PlacementData(Int2 size, PlacementRecord[,] placements)
    {
        GridSize = size;
        Placements = placements;
    }
}

[Serializable]
public struct Int2
{
    public int x;
    public int z;
    public Int2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

