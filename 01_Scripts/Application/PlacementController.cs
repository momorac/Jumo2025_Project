using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlacementRegistry registry;       // SO 에셋 드래그 할당
    [SerializeField] private FacilityMetaData facilityMeta;   // SO 에셋 드래그 할당
    [SerializeField] private PlacementSystem placementSystem;


    public void Initialize(PlacementRegistry reg, FacilityMetaData meta)
    {
        registry = reg;
        facilityMeta = meta;
    }

    public bool CanPlace(FacilityType type)
    {
        if (registry == null || facilityMeta == null) return false;
        var prefab = registry.GetPrefab(type);
        return facilityMeta.IsUnlocked(type) && prefab != null;
    }

    public GameObject Place(FacilityType type, Vector3 pos, Quaternion rot)
    {
        if (facilityMeta != null && !facilityMeta.IsUnlocked(type))
        {
            Debug.LogWarning($"Locked facility: {type}");
            return null;
        }

        if (registry == null)
        {
            Debug.LogError("FacilityRegistry is not assigned.");
            return null;
        }

        var prefab = registry.GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found for {type}.");
            return null;
        }

        var go = Instantiate(prefab, pos, rot);

        var service = go.GetComponent<IFacilityService>();
        service?.Initialize();
        service?.OnPlaced();

        return go;
    }
}
