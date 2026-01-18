using System;
using System.Collections.Generic;

[System.Serializable]
public class MetaGameData
{
    public int EconomyBalance;
    public FacilityMetaData FacilityMetaData;
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

