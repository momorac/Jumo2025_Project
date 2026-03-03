using UnityEngine;

/// <summary>
/// Staff 대기 상태. 작업 배정을 기다림
/// </summary>
public class StaffIdleState : IStaffState
{
    public StaffStateId Id => StaffStateId.Idle;

    private readonly StaffController controller;

    public StaffIdleState(StaffController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.StopMoving();
        controller.SetAnimatorBool("IsWalking", false);
        GameLogger.LogVerbose(LogCategory.Staff, $"{controller.name}: entered Idle");
    }

    public void Tick(float deltaTime)
    {
        // Idle 상태에서는 자동 배정을 기다림
        // TaskAssigner가 작업 배정 시 ChangeState 호출
    }

    public void Exit()
    {
        // 정리 작업 없음
    }
}
