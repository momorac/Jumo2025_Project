using UnityEngine;

/// <summary>
/// Staff 이동 상태
/// 목표 위치로 NavMesh 이동
/// </summary>
public class StaffMovingToTargetState : IStaffState
{
    public StaffStateId Id => StaffStateId.MovingToTarget;

    private readonly StaffController controller;
    private Vector3 targetPosition;

    public StaffMovingToTargetState(StaffController controller)
    {
        this.controller = controller;
    }

    /// <summary>이동 목표 설정</summary>
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
    }

    public void Enter()
    {
        controller.SetAnimatorBool("IsWalking", true);
        controller.SetDestination(targetPosition);
        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: moving to {targetPosition}");
    }

    public void Tick(float deltaTime)
    {
        if (controller.HasReachedDestination())
        {
            controller.OnMovementCompleted();
        }
    }

    public void Exit()
    {
        controller.SetAnimatorBool("IsWalking", false);
        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: exited MovingToTarget");
    }
}
