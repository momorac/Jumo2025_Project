using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMetaData
{
    public int EconomyBalance;
    public PlacementData PlacementData;
    public PlaceableData PlaceableData;

    public GameMetaData()
    {
        EconomyBalance = 0;
        PlacementData = null;
        PlaceableData = new PlaceableData();

        PlaceableData.Unlock(FacilityType.JumoHouse);
        PlaceableData.Unlock(FacilityType.Hearth);
    }
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

[System.Serializable]
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

[System.Serializable]
public class PlaceableData
{
    public HashSet<FacilityType> unlockedFacilities = new HashSet<FacilityType>();
    public HashSet<TileType> unlockedTiles = new HashSet<TileType>();
    public HashSet<DecorationType> unlockedDecorations = new HashSet<DecorationType>();

    public bool IsUnlocked(FacilityType type)
    {
        return this.unlockedFacilities != null && unlockedFacilities.Contains(type);
    }
    public bool IsUnlocked(TileType type)
    {
        return this.unlockedTiles != null && unlockedTiles.Contains(type);
    }
    public bool IsUnlocked(DecorationType type)
    {
        return this.unlockedDecorations != null && unlockedDecorations.Contains(type);
    }

    public bool Unlock(FacilityType type)
    {
        if (unlockedFacilities == null) unlockedFacilities = new HashSet<FacilityType>();
        return unlockedFacilities.Add(type);
    }
    public bool Unlock(TileType type)
    {
        if (unlockedTiles == null) unlockedTiles = new HashSet<TileType>();
        return unlockedTiles.Add(type);
    }
    public bool Unlock(DecorationType type)
    {
        if (unlockedDecorations == null) unlockedDecorations = new HashSet<DecorationType>();
        return unlockedDecorations.Add(type);
    }

    public IReadOnlyCollection<FacilityType> GetUnlockedFacilities()
    {
        return unlockedFacilities;
    }
    public IReadOnlyCollection<TileType> GetUnlockedTiles()
    {
        return unlockedTiles;
    }
    public IReadOnlyCollection<DecorationType> GetUnlockedDecorations()
    {
        return unlockedDecorations;
    }
}

