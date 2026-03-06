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
    private int amount;

    public StaffCarryingResourceState(StaffController controller)
    {
        this.controller = controller;
    }

    public void SetResourceType(FacilityResourceType type, int amount)
    {
        resourceType = type;
        this.amount = amount;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        controller.SetAnimatorBool("IsWalking", true);
        controller.SetDestination(targetPosition);
    }

    public void Enter()
    {
        App.EventBus.Subscribe<StaffSelectedEvent>(OnStaffSelected);

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

        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: entered CarryingResourceState with {resourceType} x{amount}");
    }

    public void Tick(float deltaTime)
    {
        if (controller.HasReachedDestination())
        {
            controller.SetAnimatorBool("IsWalking", false);
        }
    }

    public void Exit()
    {
        // 정리 작업
        resourceType = FacilityResourceType.None;
        amount = 0;

        controller.SetAnimatorBool("IsCarrying", false);
        controller.DisableAllProps();
    }

    private void OnStaffSelected(StaffSelectedEvent e)
    {
        if (controller.IsSameStaff(e.Staff))
        {
            // 자원 운반 상태에서 선택된 경우, 강제로 Idle상태로 전환 (선택 시 작업 취소)
            controller.ForceChangeToIdle();
            GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: was selected while carrying resource, forcing idle state");
        }
    }
}
