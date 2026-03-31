using UnityEngine;

/// <summary>
/// Staff 이동 상태
/// 목표 위치로 NavMesh 이동
/// </summary>
public class StaffServingState : IStaffState
{
    public StaffStateId Id => StaffStateId.MovingToTarget;

    private readonly StaffController controller;
    private Vector3 targetPosition;

    public StaffServingState(StaffController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.StopMoving();
        controller.SetAnimatorBool("IsCarrying", true);
    }

    public void Tick(float deltaTime)
    {
    }

    public void Exit()
    {
    }

    public void MoveTo(Vector3 targetPosition)
    {
        controller.SetAnimatorBool("IsWalking", true);
        controller.SetDestination(targetPosition);
    }
}
