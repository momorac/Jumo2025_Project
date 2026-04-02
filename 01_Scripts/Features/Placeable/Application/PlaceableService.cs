using System.Collections.Generic;
using UnityEngine;

public class PlaceableService
{
    private readonly PlaceableMeta placeableMeta;

    // 런타임 시설 인스턴스 관리
    private readonly Dictionary<FacilityType, List<ICookingFacility>> _cookingFacilitiesByType = new();
    private readonly Dictionary<CookingFacilityType, List<ICookingFacility>> _cookingFacilitiesByCookingType = new();
    private readonly Dictionary<FacilityResourceType, List<ResourceFacilityBase>> _resourceFacilities = new();

    public PlaceableService(PlaceableMeta placeableMeta)
    {
        this.placeableMeta = placeableMeta ?? new PlaceableMeta();
        EnsureCollectionsInitialized();
    }

    public PlaceableMeta GetMeta() => placeableMeta;

    private void EnsureCollectionsInitialized()
    {
        if (placeableMeta.ul_facility == null)
        {
            placeableMeta.ul_facility = new HashSet<FacilityType>();
        }
        if (placeableMeta.ul_tile == null)
        {
            placeableMeta.ul_tile = new HashSet<TileType>();
        }
        if (placeableMeta.ul_decoration == null)
        {
            placeableMeta.ul_decoration = new HashSet<DecorationType>();
        }
    }

    public IReadOnlyCollection<FacilityType> GetUnlockedFacilities()
    {
        return placeableMeta.ul_facility;
    }

    public IReadOnlyCollection<TileType> GetUnlockedTiles()
    {
        return placeableMeta.ul_tile;
    }

    public IReadOnlyCollection<DecorationType> GetUnlockedDecorations()
    {
        return placeableMeta.ul_decoration;
    }

    public bool IsUnlocked(FacilityType type)
    {
        return placeableMeta.ul_facility != null && placeableMeta.ul_facility.Contains(type);
    }

    public bool IsUnlocked(TileType type)
    {
        return placeableMeta.ul_tile != null && placeableMeta.ul_tile.Contains(type);
    }

    public bool IsUnlocked(DecorationType type)
    {
        return placeableMeta.ul_decoration != null && placeableMeta.ul_decoration.Contains(type);
    }

    public bool Unlock(FacilityType type)
    {
        EnsureCollectionsInitialized();
        return placeableMeta.ul_facility.Add(type);
    }

    public bool Unlock(TileType type)
    {
        EnsureCollectionsInitialized();
        return placeableMeta.ul_tile.Add(type);
    }

    public bool Unlock(DecorationType type)
    {
        EnsureCollectionsInitialized();
        return placeableMeta.ul_decoration.Add(type);
    }

    #region CookingFacility 런타임 관리

    public void RegisterCookingFacility(ICookingFacility facility)
    {
        var facilityType = facility.FacilityType;

        if (!_cookingFacilitiesByType.TryGetValue(facilityType, out var list))
        {
            list = new List<ICookingFacility>();
            _cookingFacilitiesByType[facilityType] = list;
        }
        list.Add(facility);

        var cookingType = facilityType.ToCookingType();
        if (cookingType != CookingFacilityType.None)
        {
            if (!_cookingFacilitiesByCookingType.TryGetValue(cookingType, out var cookingList))
            {
                cookingList = new List<ICookingFacility>();
                _cookingFacilitiesByCookingType[cookingType] = cookingList;
            }
            cookingList.Add(facility);
        }

        Debug.Log($"[PlaceableService] {facilityType} 등록됨 (총 {list.Count}개)");
    }

    public void UnregisterCookingFacility(ICookingFacility facility)
    {
        var facilityType = facility.FacilityType;

        if (_cookingFacilitiesByType.TryGetValue(facilityType, out var list))
        {
            list.Remove(facility);
        }

        var cookingType = facilityType.ToCookingType();
        if (cookingType != CookingFacilityType.None &&
            _cookingFacilitiesByCookingType.TryGetValue(cookingType, out var cookingList))
        {
            cookingList.Remove(facility);
        }

        Debug.Log($"[PlaceableService] {facilityType} 해제됨");
    }

    public void RegisterResourceFacility(ResourceFacilityBase facility)
    {
        var resourceType = facility.ProvidedResourceType;

        if (!_resourceFacilities.TryGetValue(resourceType, out var list))
        {
            list = new List<ResourceFacilityBase>();
            _resourceFacilities[resourceType] = list;
        }
        list.Add(facility);

        Debug.Log($"[PlaceableService] 자원 시설 {facility.FacilityType} 등록됨 ({resourceType})");
    }

    public void UnregisterResourceFacility(ResourceFacilityBase facility)
    {
        var resourceType = facility.ProvidedResourceType;

        if (_resourceFacilities.TryGetValue(resourceType, out var list))
        {
            list.Remove(facility);
        }
    }

    public IReadOnlyList<ICookingFacility> GetCookingFacilities(FacilityType type)
    {
        return _cookingFacilitiesByType.TryGetValue(type, out var list) ? list : System.Array.Empty<ICookingFacility>();
    }

    public IReadOnlyList<ICookingFacility> GetCookingFacilities(CookingFacilityType type)
    {
        return _cookingFacilitiesByCookingType.TryGetValue(type, out var list) ? list : System.Array.Empty<ICookingFacility>();
    }

    public ICookingFacility GetAvailableCookingFacility(CookingFacilityType type)
    {
        if (!_cookingFacilitiesByCookingType.TryGetValue(type, out var list))
            return null;

        foreach (var facility in list)
        {
            if (facility.CanCook)
                return facility;
        }
        return null;
    }

    public IReadOnlyList<ResourceFacilityBase> GetResourceFacilities(FacilityResourceType resourceType)
    {
        return _resourceFacilities.TryGetValue(resourceType, out var list)
            ? list
            : System.Array.Empty<ResourceFacilityBase>();
    }

    public ResourceFacilityBase GetAvailableResourceFacility(FacilityResourceType resourceType)
    {
        if (!_resourceFacilities.TryGetValue(resourceType, out var list))
            return null;

        foreach (var facility in list)
        {
            if (facility.HasResource)
                return facility;
        }
        return null;
    }

    #endregion
}
