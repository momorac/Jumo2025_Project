using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Staff 작업 대기열
/// 우선순위 및 생성 시간 기반으로 작업 관리
/// </summary>
public class TaskQueue
{
    private readonly List<IStaffTask> pendingTasks = new();
    private readonly List<IStaffTask> assignedTasks = new();

    /// <summary>대기 중인 작업 수</summary>
    public int PendingCount => pendingTasks.Count;

    /// <summary>배정된 작업 수</summary>   
    public int AssignedCount => assignedTasks.Count;

    /// <summary> 새 작업을 대기열에 추가합니다 </summary>
    public void Enqueue(IStaffTask task)
    {
        pendingTasks.Add(task);
        SortPendingTasks();

        Debug.Log($"<color=blue>Task {task.TaskId} ({task.Type}) added to queue. Pending: {PendingCount}</color>");

        // 작업 생성 이벤트 발행
        App.EventBus.Publish(new TaskCreatedEvent(task));
    }

    /// <summary>가장 우선순위가 높은 작업을 가져옵니다 (제거하지 않음)</summary>
    public IStaffTask Peek()
    {
        return pendingTasks.Count > 0 ? pendingTasks[0] : null;
    }

    /// <summary>가장 우선순위가 높은 작업을 가져오고 배정 목록으로 이동합니다</summary>
    public IStaffTask Dequeue()
    {
        if (pendingTasks.Count == 0)
            return null;

        var task = pendingTasks[0];
        pendingTasks.RemoveAt(0);
        assignedTasks.Add(task);

        return task;
    }

    /// <summary>특정 위치에 가장 가까운 작업을 가져옵니다</summary>
    public IStaffTask DequeueClosestTo(Vector3 position)
    {
        if (pendingTasks.Count == 0)
            return null;

        // 같은 우선순위 내에서 가장 가까운 작업 찾기
        int highestPriority = pendingTasks[0].Priority;
        var samePriorityTasks = pendingTasks.Where(t => t.Priority == highestPriority).ToList();

        IStaffTask closest = samePriorityTasks
            .OrderBy(t => Vector3.Distance(position, t.TargetPosition.position))
            .FirstOrDefault();

        if (closest != null)
        {
            pendingTasks.Remove(closest);
            assignedTasks.Add(closest);
        }

        return closest;
    }

    /// <summary>작업 완료 처리</summary>
    public void CompleteTask(IStaffTask task)
    {
        if (assignedTasks.Contains(task))
        {
            assignedTasks.Remove(task);
            task.Complete();
        }
    }

    /// <summary>작업 취소 처리</summary>
    public void CancelTask(IStaffTask task)
    {
        if (pendingTasks.Contains(task))
        {
            pendingTasks.Remove(task);
            task.Cancel();
        }
        else if (assignedTasks.Contains(task))
        {
            assignedTasks.Remove(task);
            task.Cancel();
        }
    }

    /// <summary>특정 Customer와 연관된 모든 작업 취소</summary>
    public void CancelTasksForCustomer(Customer customer)
    {
        var tasksToCancel = pendingTasks
            .Concat(assignedTasks)
            .Where(t => t.AssociatedCustomer == customer)
            .ToList();

        foreach (var task in tasksToCancel)
        {
            CancelTask(task);
        }
    }

    /// <summary> 대기열 정렬 (우선순위 높은 순 → 생성 시간 빠른 순) </summary>
    private void SortPendingTasks()
    {
        pendingTasks.Sort((a, b) =>
        {
            int priorityCompare = b.Priority.CompareTo(a.Priority);
            if (priorityCompare != 0)
                return priorityCompare;

            return a.CreatedTime.CompareTo(b.CreatedTime);
        });
    }

    /// <summary> 모든 대기 작업 반환 (읽기 전용) </summary>
    public IReadOnlyList<IStaffTask> GetPendingTasks() => pendingTasks.AsReadOnly();

    /// <summary> 모든 배정 작업 반환 (읽기 전용) </summary>
    public IReadOnlyList<IStaffTask> GetAssignedTasks() => assignedTasks.AsReadOnly();
}
