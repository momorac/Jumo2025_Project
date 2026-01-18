using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlacementSystem))]
public class PlacementController : MonoBehaviour, IPlacementController
{
    [Header("References")]
    [SerializeField] private PlacementRegistry registry;    // SO 에셋 드래그 할당

    private PlacementSystem placementSystem;
    private Action<PlacementRecord[,]> placementUpdatedHandler;

    public void Initialize(Action<PlacementRecord[,]> onPlacementUpdated)
    {
        if (placementSystem == null)
            placementSystem = GetComponent<PlacementSystem>();

        // 중복 구독 방지
        if (placementUpdatedHandler != null)
            placementSystem.PlacementUpdated -= placementUpdatedHandler;

        placementUpdatedHandler = onPlacementUpdated;
        if (placementUpdatedHandler != null)
            placementSystem.PlacementUpdated += placementUpdatedHandler;
    }

    public bool CanPlace(FacilityType type)
    {
        return true;
    }

    public GameObject Place(FacilityType type, Vector3 pos, Quaternion rot)
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

    public GameObject GetGameObjectPrefab(FacilityType type)
    {
        return registry.GetGameObjectPrefab(type);
    }

    public GameObject GetUiIconPrefab(FacilityType type)
    {
        return registry.GetUiIconPrefab(type);
    }

    public Vector2Int GetGridSize()
    {
        return placementSystem.GetGridSize();
    }

    private void OnDestroy()
    {
        if (placementSystem != null && placementUpdatedHandler != null)
            placementSystem.PlacementUpdated -= placementUpdatedHandler;
    }
}
