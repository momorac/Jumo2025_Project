using UnityEngine;

/// <summary>
/// Staff 작업 수행 상태
/// Task의 전체 Phase 사이클(이동→실행→다음Phase...)을 내부에서 완결 처리.
/// 모든 Phase 완료 시 Controller.OnTaskCompleted()로 신호 전달.
/// </summary>
public class StaffExecutingTaskState : IStaffState
{
    public StaffStateId Id => StaffStateId.ExecutingTask;

    private enum PhaseStep { Moving, Executing }

    private readonly StaffController controller;
    private IStaffTask task;
    private TaskPhase currentPhase;
    private PhaseStep currentStep;
    private float executionTime;
    private float elapsedTime;
    private bool phaseExecuted;

    public StaffExecutingTaskState(StaffController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        task = controller.CurrentTask;

        if (task == null || task.CurrentPhase == null)
        {
            // Task나 Phase가 없으면 다음 Tick에서 완료 처리
            task = null;
            return;
        }

        BeginCurrentPhase();
    }

    public void Tick(float deltaTime)
    {
        // Enter에서 task가 null로 설정된 경우 즉시 완료
        if (task == null)
        {
            controller.OnTaskCompleted();
            return;
        }

        switch (currentStep)
        {
            case PhaseStep.Moving:
                if (controller.HasReachedDestination())
                {
                    BeginExecution();
                }
                break;

            case PhaseStep.Executing:
                elapsedTime += deltaTime;
                if (!phaseExecuted && elapsedTime >= executionTime)
                {
                    CompleteCurrentPhase();
                }
                break;
        }
    }

    public void Exit()
    {
        controller.SetAnimatorBool("IsWalking", false);
        controller.SetAnimatorBool("IsWorking", false);
    }

    /// <summary>현재 Phase 시작. 이동 필요 시 이동부터, 아니면 바로 실행</summary>
    private void BeginCurrentPhase()
    {
        currentPhase = task.CurrentPhase;
        if (currentPhase == null)
        {
            // 안전장치: 다음 Tick에서 완료 처리
            task = null;
            return;
        }

        GameLogger.LogVerbose(LogCategory.Staff,
            $"{controller.name}: Phase {task.CurrentPhaseIndex + 1}/{task.Phases.Count} of {task.Type}");

        if (currentPhase.WillMoveFirst)
        {
            currentStep = PhaseStep.Moving;
            controller.SetAnimatorBool("IsWorking", false);
            controller.SetAnimatorBool("IsWalking", true);
            controller.SetDestination(currentPhase.MoveTarget.position);
        }
        else
        {
            BeginExecution();
        }
    }

    /// <summary>Phase 실행 시작 (이동 완료 후 또는 이동 없는 Phase)</summary>
    private void BeginExecution()
    {
        currentStep = PhaseStep.Executing;
        elapsedTime = 0f;
        phaseExecuted = false;
        executionTime = currentPhase.Duration;

        controller.SetAnimatorBool("IsWalking", false);

        // 비주얼 데이터 처리 (Phase가 선언한 데이터를 State가 실행)
        if (currentPhase.PropId != StaffPropId.None)
            controller.EnableProp(currentPhase.PropId);

        controller.SetAnimatorBool("IsWorking", true);

        if (!string.IsNullOrEmpty(currentPhase.AnimationTrigger))
            controller.SetAnimatorTrigger(currentPhase.AnimationTrigger);

        GameLogger.LogVerbose(LogCategory.Staff,
            $"{controller.name}: executing phase {task.CurrentPhaseIndex + 1} ({executionTime}s)");

        // 커스텀 로직 콜백 (필요한 경우만)
        currentPhase.OnStart?.Invoke(controller.Staff);
    }

    /// <summary>현재 Phase 실행 완료. 다음 Phase로 진행하거나 Task 완료</summary>
    private void CompleteCurrentPhase()
    {
        currentPhase.OnExecute?.Invoke(controller.Staff);
        currentPhase.OnEnd?.Invoke(controller.Staff);
        phaseExecuted = true;

        // 비주얼 정리 (데이터 기반 일괄 처리)
        controller.SetAnimatorBool("IsWorking", false);
        controller.DisableAllProps();

        if (task.AdvancePhase())
        {
            BeginCurrentPhase();
        }
        else
        {
            controller.OnTaskCompleted();
        }
    }
}
