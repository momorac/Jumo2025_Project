using System.Linq;
using UnityEngine;

public class PlacementController : MonoBehaviour, IPlacementController
{
    [Header("References")]
    [SerializeField] private PlacementRegistry registry;    // SO 에셋 드래그 할당

    private PlacementSystem placementSystem;

    public void Initialize(PlacementSystem _placementSystem)
    {
        placementSystem = _placementSystem;
    }

    public bool CanPlace(FacilityType type)
    {
        var availableFacilities = App.GameData.FacilityMetaData.GetUnlockedTypes();

        if (registry == null || availableFacilities == null) return false;
        var prefab = registry.GetPrefab(type);
        return availableFacilities.Contains(type) && prefab != null;
    }

    public GameObject Place(FacilityType type, Vector3 pos, Quaternion rot)
    {
        if (!CanPlace(type))
        {
            Debug.LogWarning($"Cannot place facility: {type}");
            return null;
        }

        var prefab = registry.GetPrefab(type);
        var go = Instantiate(prefab, pos, rot);

        var service = go.GetComponent<IFacilityService>();
        service?.Initialize();
        service?.OnPlaced();

        return go;
    }

    // public GameObject Place(FacilityType type, Vector3 pos, Quaternion rot)
    // {
    //     if (facilityMeta != null && !facilityMeta.IsUnlocked(type))
    //     {
    //         Debug.LogWarning($"Locked facility: {type}");
    //         return null;
    //     }

    //     if (registry == null)
    //     {
    //         Debug.LogError("FacilityRegistry is not assigned.");
    //         return null;
    //     }

    //     var prefab = registry.GetPrefab(type);
    //     if (prefab == null)
    //     {
    //         Debug.LogError($"Prefab not found for {type}.");
    //         return null;
    //     }

    //     var go = Instantiate(prefab, pos, rot);

    //     var service = go.GetComponent<IFacilityService>();
    //     service?.Initialize();
    //     service?.OnPlaced();

    //     return go;
    // }
}
