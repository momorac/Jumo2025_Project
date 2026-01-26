using System;
using System.Collections.Generic;

[System.Serializable]
public class GameMetaData
{
    public PlacementData PlacementData;
    public PlaceableData PlaceableData;
    public Economy EconomyData;
}

[Serializable]
public class SessionData
{
    public int availableSeats;

    public SessionData()
    {
        availableSeats = 0;
    }

    public void ModifySeats(int count)
    {
        availableSeats += count;
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

    public PlaceableData()
    {
        Unlock(FacilityType.Table);
        Unlock(FacilityType.JumoHouse);
        Unlock(FacilityType.Hearth);
    }

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

