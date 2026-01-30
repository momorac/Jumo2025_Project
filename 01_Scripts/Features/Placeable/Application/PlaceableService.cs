using System.Collections.Generic;

public class PlaceableService
{
    private readonly PlaceableData placeableData;

    public PlaceableService(PlaceableData placeableData)
    {
        this.placeableData = placeableData ?? new PlaceableData();
    }

    public IReadOnlyCollection<FacilityType> GetUnlockedFacilities()
    {
        return placeableData.GetUnlockedFacilities();
    }

    public IReadOnlyCollection<TileType> GetUnlockedTiles()
    {
        return placeableData.GetUnlockedTiles();
    }

    public IReadOnlyCollection<DecorationType> GetUnlockedDecorations()
    {
        return placeableData.GetUnlockedDecorations();
    }

    public bool IsUnlocked(FacilityType type)
    {
        return placeableData.IsUnlocked(type);
    }

    public bool IsUnlocked(TileType type)
    {
        return placeableData.IsUnlocked(type);
    }

    public bool IsUnlocked(DecorationType type)
    {
        return placeableData.IsUnlocked(type);
    }

    public bool Unlock(FacilityType type)
    {
        return placeableData.Unlock(type);
    }

    public bool Unlock(TileType type)
    {
        return placeableData.Unlock(type);
    }

    public bool Unlock(DecorationType type)
    {
        return placeableData.Unlock(type);
    }
}
