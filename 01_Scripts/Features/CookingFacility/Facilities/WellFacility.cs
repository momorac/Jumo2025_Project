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

        // 선택된 Staff 또는 가장 가까운 Idle Staff에게 Task 배정
        var staff = App.TaskAssigner.GetBestStaffFor(transform.position);
        if (staff == null)
        {
            Debug.LogWarning("배정 가능한 Staff가 없습니다");
            return;
        }

        var task = new CollectResourceTask(this, collectDuration);
        App.TaskAssigner.AssignTaskToStaff(task, staff);
    }

}
