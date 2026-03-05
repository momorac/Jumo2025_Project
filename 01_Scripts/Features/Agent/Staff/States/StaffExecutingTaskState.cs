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
        currentPhase = controller.CurrentTask?.CurrentPhase;

        if (currentPhase != null)
        {
            executionTime = currentPhase.Duration;
            GameLogger.LogVerbose(LogCategory.Staff,
                $"{controller.name}: executing phase {controller.CurrentTask.CurrentPhaseIndex + 1} ({executionTime}s)");

            controller.PlayPhaseAnimation(currentPhase);
            currentPhase.OnStart?.Invoke(controller.Staff);
        }
        else
        {
            // Phase가 없으면 즉시 완료되도록 설정 (다음 Tick에서 처리)
            executionTime = 0f;
            GameLogger.LogWarning(LogCategory.Staff,
                $"{controller.name}: entered ExecutingTask with null phase");
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
            controller.OnPhaseExecutionCompleted();
        }
    }

    public void Exit()
    {
        // 타이밍 상태만 정리. 애니메이션은 Controller가 관리
    }
}
