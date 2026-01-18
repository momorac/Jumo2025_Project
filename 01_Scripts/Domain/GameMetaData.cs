using System;
using System.Collections.Generic;
using System.Numerics;

[System.Serializable]
public class GameMetaData
{
    public int EconomyBalance;
    public PlacementMetaData PlacementMetaData;
    public FacilityMetaData FacilityMetaData;
}

[System.Serializable]
public class PlacementMetaData
{
    public Vector2 GridSize;
    public List<PlacementRecord> Placements = new List<PlacementRecord>();

    public void InitializeGrid(Vector2 size)
    {
        GridSize = size;
    }
}

[System.Serializable]
public class FacilityMetaData
{
    private List<FacilityType> unlockedFacilities = new List<FacilityType>();

    public bool IsUnlocked(FacilityType type)
    {
        return this.unlockedFacilities != null && unlockedFacilities.Contains(type);
    }

    public bool Unlock(FacilityType type)
    {
        if (unlockedFacilities == null) unlockedFacilities = new List<FacilityType>();
        if (unlockedFacilities.Contains(type)) return false;
        unlockedFacilities.Add(type);
        return true;
    }

    public IReadOnlyList<FacilityType> GetUnlockedTypes()
    {
        return unlockedFacilities;
    }
}

