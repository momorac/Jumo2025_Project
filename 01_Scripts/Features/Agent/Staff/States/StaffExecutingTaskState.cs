using UnityEngine;

/// <summary>
/// Staff 작업 수행 상태
/// 현재 Phase의 Duration 동안 대기하며, 중간 지점에서 OnExecute 호출
/// 애니메이션은 StaffController가 관리
/// </summary>
public class StaffExecutingTaskState : IStaffState
{
    public StaffStateId Id => StaffStateId.ExecutingTask;

    private readonly StaffController controller;
    private float executionTime;
    private float elapsedTime;
    private bool phaseExecuted;
    private TaskPhase currentPhase;

    public StaffExecutingTaskState(StaffController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        elapsedTime = 0f;
        phaseExecuted = false;
        currentPhase = controller.CurrentPhase;

        if (currentPhase != null)
        {
            executionTime = currentPhase.Duration;
            GameLogger.LogVerbose(LogCategory.Staff,
                $"{controller.name}: executing phase {controller.CurrentPhaseIndex + 1} ({executionTime}s)");

            controller.PlayPhaseAnimation(currentPhase);
            currentPhase.OnStart?.Invoke(controller.Staff);
        }
        else
        {
            // Phase가 없으면 바로 Phase 완료 처리
            controller.OnPhaseCompleted();
        }
    }

    public void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;

        // ExecutionTime 시간 이후 Phase 실행 완료
        if (!phaseExecuted && elapsedTime >= executionTime)
        {
            currentPhase?.OnExecute?.Invoke(controller.Staff);
            currentPhase?.OnEnd?.Invoke(controller.Staff);
            phaseExecuted = true;
            controller.OnPhaseCompleted();
        }
    }

    public void Exit()
    {
        // 타이밍 상태만 정리. 애니메이션은 Controller가 관리
    }
}
