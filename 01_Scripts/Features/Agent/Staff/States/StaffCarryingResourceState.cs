using Mono.Cecil;
using UnityEngine;

/// <summary>
/// Staff 자원 운반 상태
/// CollectResourceTask 완료 후 자원을 운반하는 상태
/// 운반 완료 시 Controller.OnResourceDelivered()로 신호 전달.
/// </summary>
public class StaffCarryingResourceState : IStaffState
{
    public StaffStateId Id => StaffStateId.CarryingResource;

    private readonly StaffController controller;

    private FacilityResourceType resourceType;

    public StaffCarryingResourceState(StaffController controller)
    {
        this.controller = controller;
    }

    public void SetResourceType(FacilityResourceType type)
    {
        resourceType = type;
    }


    public void Enter()
    {
        controller.StopMoving();
        controller.SetAnimatorBool("IsCarrying", true);

        if (resourceType == FacilityResourceType.Water)
        {
            controller.EnableProp(StaffPropId.WaterBucket);
        }
        else if (resourceType == FacilityResourceType.Firewood)
        {
            controller.EnableProp(StaffPropId.Firewood);
        }

        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: entered CarryingResourceState");
    }

    public void Tick(float deltaTime)
    {
    }

    public void Exit()
    {
        // 정리 작업
        controller.SetAnimatorBool("IsCarrying", false);
        controller.DisableAllProps();
    }
}
