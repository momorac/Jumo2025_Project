using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMetaData
{
    public int EconomyBalance;
    public PlacementMetaData PlacementMetaData;
    public FacilityMetaData FacilityMetaData;

    public GameMetaData()
    {
        EconomyBalance = 0;
        PlacementMetaData = null;
        FacilityMetaData = new FacilityMetaData();

        FacilityMetaData.Unlock(FacilityType.JumoHouse);
        FacilityMetaData.Unlock(FacilityType.Hearth);
    }
}

[System.Serializable]
public class PlacementMetaData
{
    public Vector2Int GridSize;
    public PlacementRecord[,] Placements;

    public PlacementMetaData(Vector2Int size, PlacementRecord[,] placements)
    {
        GridSize = size;
        Placements = placements;
    }
}

[System.Serializable]
public class FacilityMetaData
{
    public List<FacilityType> unlockedFacilities = new List<FacilityType>();

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

