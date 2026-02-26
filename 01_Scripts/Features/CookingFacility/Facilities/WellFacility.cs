using UnityEngine;

/// <summary> 우물 - 물 공급 시설 </summary>
public class WellFacility : ResourceFacilityBase
{
    [Header("Well Specific")]
    [SerializeField] private float waterDrawTime = 2f;

    private void Awake()
    {
        facilityType = FacilityType.Well;
        providedResourceType = FacilityResourceType.Water;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=cyan>우물 클릭됨 - 물 {currentResource}/{maxResource}</color>");
        // TODO: 직원에게 물 길어오기 태스크 할당
    }

    /// <summary> 물 길어오기 소요 시간 </summary>
    public float WaterDrawTime => waterDrawTime;
}
