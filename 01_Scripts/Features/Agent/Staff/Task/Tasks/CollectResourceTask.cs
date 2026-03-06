using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자원 수집 작업 (1 Phase)
/// 자원 시설(우물/장작더미)로 이동 → 수집 모션 → 자원 획득
/// </summary>
public class CollectResourceTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CollectResource;

    public ResourceFacilityBase SourceFacility { get; }
    public FacilityResourceType ResourceType { get; }
    private int amount;
    public int Amount => amount;

    public CollectResourceTask(ResourceFacilityBase facility, int priority = 7)
        : base(priority)
    {
        SourceFacility = facility;
        ResourceType = facility.ProvidedResourceType;
        amount = 0;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: SourceFacility.TargetTransform,
            duration: SourceFacility.CollectDuration,
            animationTrigger: SourceFacility.FacilityType == FacilityType.Well? "CollectWater" : "CollectFirewood",
            propId: SourceFacility.FacilityType == FacilityType.Stump ? StaffPropId.Axe : StaffPropId.None,
            onStart: (controller) =>
            {
                // 회전 설정 (수집 위치를 바라보도록)
                controller.StopMoving();
                Quaternion targetRotation = Quaternion.LookRotation(SourceFacility.transform.position - controller.transform.position);
                targetRotation *= Quaternion.Euler(0f, -90f, 0f);
                controller.SetPositionAndRotation(controller.transform.position, targetRotation);
            },
            onExecute: (controller) =>
            {
                amount = SourceFacility.CollectResource();
            }
        )
    };
}
