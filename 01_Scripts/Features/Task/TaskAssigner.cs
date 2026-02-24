using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Staff 작업 배정 시스템
/// 자동 배정 + 수동 지정 지원
/// </summary>
public class TaskAssigner
{
    private readonly TaskQueue taskQueue;
    private readonly StaffRegistry staffRegistry;

    public TaskAssigner(TaskQueue taskQueue, StaffRegistry staffRegistry)
    {
        this.taskQueue = taskQueue;
        this.staffRegistry = staffRegistry;

        // 이벤트 구독
        App.EventBus.Subscribe<TaskCreatedEvent>(OnTaskCreated);
        App.EventBus.Subscribe<DestinationClickedEvent>(OnDestinationClicked);
        App.EventBus.Subscribe<BubbleClickedEvent>(OnBubbleClicked);
    }

    /// <summary>
    /// 이벤트 구독 해제
    /// </summary>
    public void Dispose()
    {
        App.EventBus.Unsubscribe<TaskCreatedEvent>(OnTaskCreated);
        App.EventBus.Unsubscribe<DestinationClickedEvent>(OnDestinationClicked);
        App.EventBus.Unsubscribe<BubbleClickedEvent>(OnBubbleClicked);
    }

    /// <summary>
    /// 작업 생성 시 자동 배정 시도
    /// </summary>
    private void OnTaskCreated(TaskCreatedEvent evt)
    {
        TryAutoAssign();
    }

    /// <summary>
    /// 말풍선 클릭 시 TakeOrder 작업 생성 및 배정
    /// </summary>
    private void OnBubbleClicked(BubbleClickedEvent evt)
    {
        Debug.Log($"<color=magenta>Bubble clicked for customer at {evt.TargetPosition.position}</color>");

        // 선택된 Staff가 있으면 해당 Staff에게 배정
        // 없으면 가장 가까운 Idle Staff에게 배정
        Staff targetStaff = GetBestStaffFor(evt.TargetPosition.position);

        if (targetStaff != null)
        {
            // TakeOrder 작업 생성
            var task = new TakeOrderTask(
                evt.Customer,
                evt.TargetPosition,
                evt.Customer.CurrentOrder,
                priority: 10
            );

            AssignTaskToStaff(task, targetStaff);
        }
        else
        {
            Debug.LogWarning("No available staff to take order!");
        }
    }

    /// <summary>
    /// 수동 목적지 클릭 시 처리
    /// </summary>
    private void OnDestinationClicked(DestinationClickedEvent evt)
    {
        // 선택된 Staff가 있으면 해당 위치로 이동 명령
        var selectedStaff = staffRegistry.GetSelectedStaff();
        if (selectedStaff != null)
        {
            selectedStaff.MoveTo(evt.Position);
            staffRegistry.ClearSelection();
        }
        else
        {
            // 선택된 Staff가 없으면 기본 Staff(주모) 이동
            var defaultStaff = staffRegistry.GetDefaultStaff();
            defaultStaff?.MoveTo(evt.Position);
        }
    }

    /// <summary>
    /// Idle 상태인 Staff에게 대기 중인 작업 자동 배정
    /// </summary>
    public void TryAutoAssign()
    {
        var idleStaffs = staffRegistry.GetIdleStaffs();

        foreach (var staff in idleStaffs)
        {
            var task = taskQueue.DequeueClosestTo(staff.transform.position);
            if (task != null)
            {
                AssignTaskToStaff(task, staff);
            }
        }
    }

    /// <summary>
    /// 특정 위치에 대해 가장 적합한 Staff 반환
    /// (가장 가까운 Idle Staff)
    /// </summary>
    public Staff GetBestStaffFor(Vector3 targetPosition)
    {
        // 먼저 선택된 Staff 확인
        var selectedStaff = staffRegistry.GetSelectedStaff();
        if (selectedStaff != null && selectedStaff.IsIdle)
        {
            return selectedStaff;
        }

        // 가장 가까운 Idle Staff 찾기
        var idleStaffs = staffRegistry.GetIdleStaffs();
        if (idleStaffs.Count == 0)
            return null;

        return idleStaffs
            .OrderBy(s => Vector3.Distance(s.transform.position, targetPosition))
            .FirstOrDefault();
    }

    /// <summary>
    /// 특정 Staff에게 작업 배정
    /// </summary>
    public void AssignTaskToStaff(IStaffTask task, Staff staff)
    {
        staff.AssignTask(task);

        App.EventBus.Publish(new TaskAssignedEvent(task, staff));

        Debug.Log($"<color=green>Task {task.TaskId} ({task.Type}) assigned to {staff.name}</color>");
    }

    /// <summary>
    /// 수동으로 특정 Staff에게 작업 배정
    /// </summary>
    public void ManualAssign(Staff staff, IStaffTask task)
    {
        // 대기열에서 제거
        if (taskQueue.GetPendingTasks().Contains(task))
        {
            // 위치 기반이 아닌 직접 제거 필요
            var pendingTask = taskQueue.Dequeue();
            while (pendingTask != null && pendingTask != task)
            {
                taskQueue.Enqueue(pendingTask); // 다시 넣기
                pendingTask = taskQueue.Dequeue();
            }
        }

        AssignTaskToStaff(task, staff);
    }
}
