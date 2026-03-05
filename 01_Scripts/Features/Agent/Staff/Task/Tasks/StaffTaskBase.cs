using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Staff 작업 기본 클래스
/// 공통 기능을 구현하고 구체적인 작업은 상속하여 구현.
/// 각 Task는 BuildPhases()를 오버라이드하여 실행 단계를 정의.
/// </summary>
public abstract class StaffTaskBase : IStaffTask
{
    private static int nextTaskId = 1;

    public int TaskId { get; }
    public abstract TaskType Type { get; }
    public int Priority { get; protected set; }
    public float CreatedTime { get; }
    public Customer AssociatedCustomer { get; protected set; }
    public OrderData AssociatedOrder { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public bool IsCancelled { get; protected set; }

    // Phases 지연 초기화 (base 생성자 → 하위 필드 미초기화 문제 방지)
    private IReadOnlyList<TaskPhase> _phases;
    public IReadOnlyList<TaskPhase> Phases => _phases ??= BuildPhases();

    // Phase 진행 상태
    private int _currentPhaseIndex;
    public int CurrentPhaseIndex => _currentPhaseIndex;
    public TaskPhase CurrentPhase => Phases != null && _currentPhaseIndex < Phases.Count
        ? Phases[_currentPhaseIndex] : null;
    public bool IsAllPhasesCompleted => Phases == null || _currentPhaseIndex >= Phases.Count;

    public bool AdvancePhase()
    {
        _currentPhaseIndex++;
        return !IsAllPhasesCompleted;
    }

    public void ResetPhaseProgress()
    {
        _currentPhaseIndex = 0;
    }

    protected StaffTaskBase(int priority = 0)
    {
        TaskId = nextTaskId++;
        Priority = priority;
        CreatedTime = Time.time;
        IsCompleted = false;
        IsCancelled = false;
    }

    /// <summary>작업의 실행 단계를 정의. 하위 클래스에서 오버라이드</summary>
    protected abstract List<TaskPhase> BuildPhases();

    public virtual void Complete()
    {
        IsCompleted = true;
        GameLogger.Log(LogCategory.Task, $"Task {TaskId} ({Type}) completed");
    }

    public virtual void Cancel()
    {
        IsCancelled = true;
        GameLogger.LogWarning(LogCategory.Task, $"Task {TaskId} ({Type}) cancelled");
    }
}
