using System.Collections.Generic;

public class PlaceableService
{
    private readonly PlaceableData placeableData;

    public PlaceableService(PlaceableData placeableData)
    {
        this.placeableData = placeableData ?? new PlaceableData();
        EnsureCollectionsInitialized();
    }

    private void EnsureCollectionsInitialized()
    {
        if (placeableData.unlockedFacilities == null)
        {
            placeableData.unlockedFacilities = new HashSet<FacilityType>();
        }
        if (placeableData.unlockedTiles == null)
        {
            placeableData.unlockedTiles = new HashSet<TileType>();
        }
        if (placeableData.unlockedDecorations == null)
        {
            placeableData.unlockedDecorations = new HashSet<DecorationType>();
        }
    }

    public IReadOnlyCollection<FacilityType> GetUnlockedFacilities()
    {
        return placeableData.unlockedFacilities;
    }

    public IReadOnlyCollection<TileType> GetUnlockedTiles()
    {
        return placeableData.unlockedTiles;
    }

    public IReadOnlyCollection<DecorationType> GetUnlockedDecorations()
    {
        return placeableData.unlockedDecorations;
    }

    public bool IsUnlocked(FacilityType type)
    {
        return placeableData.unlockedFacilities != null && placeableData.unlockedFacilities.Contains(type);
    }

    public bool IsUnlocked(TileType type)
    {
        return placeableData.unlockedTiles != null && placeableData.unlockedTiles.Contains(type);
    }

    public bool IsUnlocked(DecorationType type)
    {
        return placeableData.unlockedDecorations != null && placeableData.unlockedDecorations.Contains(type);
    }

    public bool Unlock(FacilityType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.unlockedFacilities.Add(type);
    }

    public bool Unlock(TileType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.unlockedTiles.Add(type);
    }

    public bool Unlock(DecorationType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.unlockedDecorations.Add(type);
    }
}
