using UnityEngine;

/// <summary>
/// Staff 작업 수행 상태
/// 도착 후 작업 실행
/// </summary>
public class StaffExecutingTaskState : IStaffState
{
    public StaffStateId Id => StaffStateId.ExecutingTask;

    private readonly StaffController controller;
    private float executionTime;
    private float elapsedTime;
    private bool taskExecuted;

    public StaffExecutingTaskState(StaffController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        elapsedTime = 0f;
        taskExecuted = false;

        var task = controller.CurrentTask;
        if (task != null)
        {
            // 작업 타입에 따른 실행 시간 설정
            executionTime = GetExecutionTime(task.Type);
            Debug.Log($"<color=gray>{controller.name}: Executing {task.Type} (duration: {executionTime}s)</color>");

            // 작업 시작 애니메이션
            PlayTaskAnimation(task.Type);
        }
        else
        {
            // 작업이 없으면 바로 Idle로
            controller.ChangeState(StaffStateId.Idle);
        }
    }

    public void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;

        // 실행 시간 중간에 작업 로직 실행
        if (!taskExecuted && elapsedTime >= executionTime * 0.5f)
        {
            var task = controller.CurrentTask;
            task?.Execute(controller.Staff);
            taskExecuted = true;
        }

        // 작업 완료
        if (elapsedTime >= executionTime)
        {
            CompleteTask();
        }
    }

    public void Exit()
    {
        // 애니메이션 복원
        controller.SetAnimation("IsWorking", false);
    }

    private void CompleteTask()
    {
        var task = controller.CurrentTask;
        if (task != null)
        {
            App.TaskQueue.CompleteTask(task);
            App.EventBus.Publish(new TaskCompletedEvent(task, controller.Staff));
        }

        controller.ClearCurrentTask();
        controller.ChangeState(StaffStateId.Idle);
    }

    private float GetExecutionTime(TaskType taskType)
    {
        return taskType switch
        {
            TaskType.TakeOrder => 2f,
            TaskType.ServeDrink => 1.5f,
            TaskType.ServeFood => 2f,
            TaskType.CleanTable => 3f,
            TaskType.Checkout => 2f,
            TaskType.CollectResource => GetCollectResourceTime(),
            _ => 1f
        };
    }

    private float GetCollectResourceTime()
    {
        if (controller.CurrentTask is CollectResourceTask collectTask)
            return collectTask.CollectDuration;
        return 3f;
    }

    private void PlayTaskAnimation(TaskType taskType)
    {
        // 작업 타입에 따른 애니메이션 재생
        // TODO: 각 작업별 세부 애니메이션 추가
        controller.SetAnimation("IsWorking", true);
    }
}
