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
            onStart: staff =>
            {
                staff.Controller.SetCharacterDirection(SourceFacility.transform);
                if (SourceFacility.FacilityType == FacilityType.Stump)
                    staff.Controller.ActivateProp(StaffPropId.Axe);
                staff.Controller.SetAnimatorBool("IsWorking", true);
                staff.Controller.SetAnimatorTrigger(
                    SourceFacility.FacilityType == FacilityType.Well ? "CollectWater" : "CollectFirewood");
            },
            onExecute: staff =>
            {
                int amount = SourceFacility.CollectResource();
                staff.PickUpResource(ResourceType, amount);
                GameLogger.Log(LogCategory.Task, $"{staff.name}: {ResourceType} x{amount} collected");
            },
            onEnd: staff =>
            {
                staff.Controller.SetAnimatorBool("IsWorking", false);
                staff.Controller.DeactivateAllProps();
            }
        )
    };
}
