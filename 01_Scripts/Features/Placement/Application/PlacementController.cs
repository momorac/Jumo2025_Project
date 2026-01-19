using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlacementSystem))]
public class PlacementController : MonoBehaviour, IPlacementController
{
    [Header("References")]
    [SerializeField] private PlacementRegistry registry;    // SO 에셋 드래그 할당

    private PlacementSystem placementSystem;

    public void Initialize()
    {
        if (placementSystem == null)
            placementSystem = GetComponent<PlacementSystem>();
    }

    public bool CanPlace(Placeable type)
    {
        return true;
    }

    public GameObject Place(Placeable type, Vector3 pos, Quaternion rot)
    {
        // if (!CanPlace(type))
        // {
        //     Debug.LogWarning($"Cannot place facility: {type}");
        //     return null;
        // }

        // var prefab = registry.GetPrefab(type);

        // // 권장: PlacementSystem이 그리드 검증/스냅/레코드 갱신/Instantiate/이벤트를 담당
        // if (placementSystem.TryPlace(prefab, type, pos, rot, out var go))
        // {
        //     var service = go.GetComponent<IFacilityService>();
        //     service?.Initialize();
        //     service?.OnPlaced();
        //     return go;
        // }

        // Debug.LogWarning($"Placement failed at {pos} for {type}");
        return null;
    }

    public void StartPlacing(Placeable placeable)
    {
        GameObject prefab = GetGameObjectPrefab(placeable);
        placementSystem.StartPlacing(placeable, prefab);
    }


    public Int2 GetGridSize()
    {
        return placementSystem.GetGridSize();
    }

    public IReadOnlyCollection<FacilityType> GetAvailableFacilities()
    {
        return App.GameData.PlaceableData.GetUnlockedFacilities();
    }
    public IReadOnlyCollection<TileType> GetAvailableTiles()
    {
        return App.GameData.PlaceableData.GetUnlockedTiles();
    }
    public IReadOnlyCollection<DecorationType> GetAvailableDecorations()
    {
        return App.GameData.PlaceableData.GetUnlockedDecorations();
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
            Debug.LogWarning($"Unknown placeable type: {type}");
            return null;
        }
    }
    public GameObject GetGameObjectPrefab(FacilityType facilityType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(facilityType);
        if (prefab == null)
        {
            Debug.LogWarning($"GameObject prefab missing for placeable {facilityType}");
        }
        return prefab;
    }
    public GameObject GetGameObjectPrefab(TileType tileType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(tileType);
        if (prefab == null)
        {
            Debug.LogWarning($"GameObject prefab missing for placeable {tileType}");
        }
        return prefab;
    }
    public GameObject GetGameObjectPrefab(DecorationType decorationType)
    {
        GameObject prefab = registry.GetGameObjectPrefab(decorationType);
        if (prefab == null)
        {
            Debug.LogWarning($"GameObject prefab missing for placeable {decorationType}");
        }
        return prefab;
    }

    public Sprite GetUiIcon(FacilityType facilityType)
    {
        Sprite icon = registry.GetUiIcon(facilityType);
        if (icon == null)
        {
            Debug.LogWarning($"UI Icon prefab missing for placeable {facilityType}");
        }
        return icon;
    }
    public Sprite GetUiIcon(TileType tileType)
    {
        Sprite icon = registry.GetUiIcon(tileType);
        if (icon == null)
        {
            Debug.LogWarning($"UI Icon prefab missing for placeable {tileType}");
        }
        return icon;
    }
    public Sprite GetUiIcon(DecorationType decorationType)
    {
        Sprite icon = registry.GetUiIcon(decorationType);
        if (icon == null)
        {
            Debug.LogWarning($"UI Icon prefab missing for placeable {decorationType}");
        }
        return icon;
    }
}
