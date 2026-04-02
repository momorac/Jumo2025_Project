using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMetaData
{
    public PlaceableMeta PlaceableMeta;
    public PlacementMeta PlacementMeta;
    public EconomyMeta EconomyMeta;
    public IngredientMeta IngredientMeta;
    public RecipeMeta RecipeMeta;
}

[Serializable]
public class SessionMeta
{
    public Dictionary<Seat, bool> Seats = new Dictionary<Seat, bool>();
    public int AvailableSeatsCount;
}

[Serializable]
public class PlaceableMeta
{
    public HashSet<FacilityType> ul_facility = new HashSet<FacilityType>();
    public HashSet<TileType> ul_tile = new HashSet<TileType>();
    public HashSet<DecorationType> ul_decoration = new HashSet<DecorationType>();
}

[Serializable]
public class PlacementMeta
{
    public Int2 GridSize;
    public PlacementRecord[,] Placements;

    public PlacementMeta() { }

    public PlacementMeta(Int2 size, PlacementRecord[,] placements)
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

