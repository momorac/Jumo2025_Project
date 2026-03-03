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
    private bool hasTask;

    public StaffMovingToTargetState(StaffController controller)
    {
        this.controller = controller;
    }

    /// <summary>이동 목표 설정</summary>
    public void SetTarget(Vector3 position, bool withTask = false)
    {
        targetPosition = position;
        hasTask = withTask;
    }

    public void Enter()
    {
        controller.SetAnimatorBool("IsWalking", true);
        controller.SetDestination(targetPosition);
        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: moving to {targetPosition}");
    }

    public void Tick(float deltaTime)
    {
        // 도착 확인
        if (controller.HasReachedDestination())
        {
            if (hasTask && controller.CurrentTask != null)
            {
                // 작업 수행 상태로 전환
                controller.ChangeState(StaffStateId.ExecutingTask);
            }
            else
            {
                // 작업 없이 이동만 했으면 Idle로
                controller.ChangeState(StaffStateId.Idle);
            }
        }
    }

    public void Exit()
    {
        controller.SetAnimatorBool("IsWalking", false);
        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: exited MovingToTarget");
    }
}
