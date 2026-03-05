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

    public CollectResourceTask(ResourceFacilityBase facility, float collectDuration, int priority = 7)
        : base(priority)
    {
        SourceFacility = facility;
        ResourceType = facility.ProvidedResourceType;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: SourceFacility.transform,
            duration: SourceFacility.CollectDuration,
            animationTrigger: SourceFacility.FacilityType == FacilityType.Well? "CollectWater" : "CollectFirewood",
            propId: SourceFacility.FacilityType == FacilityType.Stump ? StaffPropId.Axe : StaffPropId.None,
            onStart: (controller) =>
            {
                // 회전 설정 (데이터 필드로 표현하기 어려운 커스텀 로직)
                Quaternion targetRotation = Quaternion.LookRotation(SourceFacility.transform.position - controller.transform.position);
                controller.SetPositionAndRotation(controller.transform.position, targetRotation);
            },
            onExecute: (controller) =>
            {
                int amount = SourceFacility.CollectResource();
                controller.PickUpResource(ResourceType, amount);
                GameLogger.Log(LogCategory.Task, $"{controller.name}: {ResourceType} x{amount} collected");
            }
        )
    };
}
