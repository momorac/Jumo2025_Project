using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(PlacementSystem))]
public class PlacementController : MonoBehaviour, IPlacementController
{
    private PlacementRegistry registry;
    private PlacementSystem placementSystem;

    public void Initialize()
    {
        placementSystem = GetComponent<PlacementSystem>();

        AsyncOperationHandle handle = Addressables.LoadAssetAsync<PlacementRegistry>("Assets/_Project/91_Data/PlacementRegistry.asset");
        handle.Completed += (op) =>
        {
            registry = op.Result as PlacementRegistry;
            placementSystem.Initialize(registry);
        };
    }

    public bool CanPlace(Placeable type)
    {
        return true;
    }


    public void StartPlacing(Placeable placeable)
    {
        GameObject prefab = GetGameObjectPrefab(placeable);
        placementSystem.StartPlacing(placeable, prefab);
    }

    public IReadOnlyCollection<FacilityType> GetAvailableFacilities()
    {
        return App.PlaceableService.GetUnlockedFacilities();
    }
    public IReadOnlyCollection<TileType> GetAvailableTiles()
    {
        return App.PlaceableService.GetUnlockedTiles();
    }
    public IReadOnlyCollection<DecorationType> GetAvailableDecorations()
    {
        return App.PlaceableService.GetUnlockedDecorations();
    }

    public string GetDisplayName(FacilityType facilityType)
    {
        return registry.GetDisplayName(facilityType);
    }
    public string GetDisplayName(TileType tileType)
    {
        return registry.GetDisplayName(tileType);
    }
    public string GetDisplayName(DecorationType decorationType)
    {
        return registry.GetDisplayName(decorationType);
    }

    public GameObject GetGameObjectPrefab(Placeable type)
    {
        if (type is Facility facilityType)
        {
            return GetGameObjectPrefab(facilityType.Type);
        }
        else if (type is Tile tileType)
        {
            return GetGameObjectPrefab(tileType.Type);
        }
        else if (type is Decoration decorationType)
        {
            return GetGameObjectPrefab(decorationType.Type);
        }
        else
        {
            GameLogger.LogWarning(LogCategory.System, $"Unknown placeable type: {type}");
            return null;
        }
    }
    public GameObject GetGameObjectPrefab(FacilityType facilityType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(facilityType);
        if (prefab == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"GameObject prefab missing for placeable {facilityType}");
        }
        return prefab;
    }
    public GameObject GetGameObjectPrefab(TileType tileType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(tileType);
        if (prefab == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"GameObject prefab missing for placeable {tileType}");
        }
        return prefab;
    }
    public GameObject GetGameObjectPrefab(DecorationType decorationType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(decorationType);
        if (prefab == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"GameObject prefab missing for placeable {decorationType}");
        }
        return prefab;
    }

    public Sprite GetUiIcon(FacilityType facilityType)
    {
        Sprite icon = registry.GetUiIcon(facilityType);
        if (icon == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"UI Icon prefab missing for placeable {facilityType}");
        }
        return icon;
    }
    public Sprite GetUiIcon(TileType tileType)
    {
        Sprite icon = registry.GetUiIcon(tileType);
        if (icon == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"UI Icon prefab missing for placeable {tileType}");
        }
        return icon;
    }
    public Sprite GetUiIcon(DecorationType decorationType)
    {
        Sprite icon = registry.GetUiIcon(decorationType);
        if (icon == null)
        {
            GameLogger.LogWarning(LogCategory.System, $"UI Icon prefab missing for placeable {decorationType}");
        }
        return icon;
    }
}
