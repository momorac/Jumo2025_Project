using System;
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

    private CookingFacilityBase targetFacility;
    private event Action MoveFinished;

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
        App.EventBus.Subscribe<CookingFacilityClickedEvent>(OnCookingFacilityClicked);

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
            MoveFinished?.Invoke();
        }
    }

    public void Exit()
    {
        App.EventBus.Unsubscribe<StaffSelectedEvent>(OnStaffSelected);
        App.EventBus.Unsubscribe<CookingFacilityClickedEvent>(OnCookingFacilityClicked);

        // 정리 작업
        resourceType = FacilityResourceType.None;
        amount = 0;

        controller.SetAnimatorBool("IsCarrying", false);
        controller.DisableAllProps();
    }

    private void FillResource()
    {
        if (resourceType == FacilityResourceType.Water)
        {
            targetFacility.AddWater(amount);
        }
        else if (resourceType == FacilityResourceType.Firewood)
        {
            targetFacility.AddWood(amount);
        }

        targetFacility = null;
        MoveFinished -= FillResource;
        controller.ForceChangeToIdle();
    }

    private void OnStaffSelected(StaffSelectedEvent e)
    {
        if (controller.IsSameStaff(e.Staff))
        {
            // 자원 운반 상태에서 선택된 경우, 강제로 Idle상태로 전환 (선택 시 작업 취소)
            GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: was selected while carrying resource, forcing idle state");
            controller.ForceChangeToIdle();
        }
    }

    private void OnCookingFacilityClicked(CookingFacilityClickedEvent e)
    {
        if (e.Facility is CookingFacilityBase facility)
        {
            if (resourceType == FacilityResourceType.Water && !facility.IsWaterNeeded)
            {
                GameLogger.LogWarning(LogCategory.Staff, $"{controller.name}: clicked facility does not need water");
            }
            else if (resourceType == FacilityResourceType.Firewood && !facility.IsWoodNeeded)
            {
                GameLogger.LogWarning(LogCategory.Staff, $"{controller.name}: clicked facility does not need firewood");
            }
            else
            {
                targetFacility = facility;
                MoveTo(facility.TargetTransform.position);
                MoveFinished += FillResource;
            }
        }
        else
        {
            GameLogger.LogWarning(LogCategory.Staff, $"{controller.name}: clicked facility is not a CookingFacilityBase");
        }
    }
}
