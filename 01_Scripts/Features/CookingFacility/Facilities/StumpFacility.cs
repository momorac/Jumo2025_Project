using UnityEngine;

/// <summary> 장작더미 - 장작 공급 시설 </summary>
public class StumpFacility : ResourceFacilityBase
{
    [Header("Stump Specific")]
    [SerializeField] private float chopTime = 1.5f;

    private void Awake()
    {
        facilityType = FacilityType.Stump;
        providedResourceType = FacilityResourceType.Firewood;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=brown>장작더미 클릭됨 - 장작 {currentResource}/{maxResource}</color>");
        // TODO: 직원에게 장작 패기 태스크 할당
    }

    /// <summary> 장작 패기 소요 시간 </summary>
    public float ChopTime => chopTime;
}
