using UnityEngine;

/// <summary> 우물 - 물 공급 시설 </summary>
public class WellFacility : ResourceFacilityBase
{
    private void Awake()
    {
        facilityType = FacilityType.Well;
        providedResourceType = FacilityResourceType.Water;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=cyan>우물 클릭됨 - 물 {amountPerCollect}씩 공급</color>");
        // TODO: 직원에게 물 길어오기 태스크 할당
    }

}
