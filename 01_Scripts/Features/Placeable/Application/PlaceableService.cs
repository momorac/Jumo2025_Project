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
        if (placeableData.ul_facility == null)
        {
            placeableData.ul_facility = new HashSet<FacilityType>();
        }
        if (placeableData.ul_tile == null)
        {
            placeableData.ul_tile = new HashSet<TileType>();
        }
        if (placeableData.ul_decoration == null)
        {
            placeableData.ul_decoration = new HashSet<DecorationType>();
        }
    }

    public IReadOnlyCollection<FacilityType> GetUnlockedFacilities()
    {
        return placeableData.ul_facility;
    }

    public IReadOnlyCollection<TileType> GetUnlockedTiles()
    {
        return placeableData.ul_tile;
    }

    public IReadOnlyCollection<DecorationType> GetUnlockedDecorations()
    {
        return placeableData.ul_decoration;
    }

    public bool IsUnlocked(FacilityType type)
    {
        return placeableData.ul_facility != null && placeableData.ul_facility.Contains(type);
    }

    public bool IsUnlocked(TileType type)
    {
        return placeableData.ul_tile != null && placeableData.ul_tile.Contains(type);
    }

    public bool IsUnlocked(DecorationType type)
    {
        return placeableData.ul_decoration != null && placeableData.ul_decoration.Contains(type);
    }

    public bool Unlock(FacilityType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.ul_facility.Add(type);
    }

    public bool Unlock(TileType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.ul_tile.Add(type);
    }

    public bool Unlock(DecorationType type)
    {
        EnsureCollectionsInitialized();
        return placeableData.ul_decoration.Add(type);
    }
}
