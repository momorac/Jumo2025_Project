using UnityEngine;

/// <summary>
/// Staff 작업 수행 상태
/// 현재 Phase의 Duration 동안 대기하며, 중간 지점에서 OnExecute 호출
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

            // Phase별 애니메이션 재생
            PlayPhaseAnimation(currentPhase);
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

        // 실행 시간 중간 지점에서 Phase 로직 실행
        if (!phaseExecuted && elapsedTime >= executionTime * 0.5f)
        {
            currentPhase?.OnExecute?.Invoke(controller.Staff);
            phaseExecuted = true;
        }

        // Phase 실행 완료
        if (elapsedTime >= executionTime)
        {
            controller.OnPhaseCompleted();
        }
    }

    public void Exit()
    {
        // 애니메이션 복원
        controller.SetAnimation("IsWorking", false);
    }

    private void PlayPhaseAnimation(TaskPhase phase)
    {
        if (!string.IsNullOrEmpty(phase.AnimationTrigger))
        {
            controller.TriggerAnimation(phase.AnimationTrigger);
        }

        controller.SetAnimation("IsWorking", true);
    }
}
