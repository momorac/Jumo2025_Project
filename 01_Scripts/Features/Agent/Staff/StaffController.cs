using System.Collections.Generic;
using UnityEngine;

public enum StaffPropId
{
    None,
    Tray,        // 쟁반 (서빙 작업 시 등장)
    CleaningTool, // 청소 도구 (청소 작업 시 등장)
    Axe,          // 도끼 (장작 수집 시 등장)
    WaterBucket,         // 양동이 (물 수집 시 등장)
    Firewood,      // 장작 (장작 수집 후 등장)
}

[System.Serializable]
public class StaffProp
{
    public StaffPropId Id;
    public GameObject GameObject;
}

/// <summary>
/// Staff FSM 컨트롤러
/// 상태 전환 및 현재 작업 관리
/// </summary>
public class StaffController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Props")]
    [SerializeField] private List<StaffProp> props;


    // FSM
    private Dictionary<StaffStateId, IStaffState> states;
    private IStaffState currentState;

    // 참조
    private Staff staff;
    private IStaffTask currentTask;

    // 자원 운반 상태
    private FacilityResourceType carryingResourceType = FacilityResourceType.None;
    private int carryingAmount = 0;

    public IStaffTask CurrentTask => currentTask;
    public StaffStateId CurrentStateId => currentState?.Id ?? StaffStateId.Idle;
    public bool IsIdle => CurrentStateId == StaffStateId.Idle;

    // 자원 운반 상태 관련
    public bool IsCarrying => carryingResourceType != FacilityResourceType.None;
    public FacilityResourceType CarryingResourceType => carryingResourceType;
    public int CarryingAmount => carryingAmount;

    private void Awake()
    {
        staff = GetComponent<Staff>();


        InitializeStates();
        ChangeState(StaffStateId.Idle);
    }

    private void Start()
    {
        // 초기 상태 설정
        ChangeState(StaffStateId.Idle);

        // Registry에 등록
        App.StaffRegistry.Register(staff);

        // 초기 Prop 비활성화
        DisableAllProps();
    }

    private void OnDestroy()
    {
        if (App.StaffRegistry != null)
        {
            App.StaffRegistry.Unregister(staff);
        }
    }

    private void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    private void InitializeStates()
    {
        states = new Dictionary<StaffStateId, IStaffState>
        {
            { StaffStateId.Idle, new StaffIdleState(this) },
            { StaffStateId.MovingToTarget, new StaffMovingToTargetState(this) },
            { StaffStateId.ExecutingTask, new StaffExecutingTaskState(this) },
            { StaffStateId.CarryingResource, new StaffCarryingResourceState(this) }
        };
    }

    public IStaffState ChangeState(StaffStateId newStateId)
    {
        if (!states.ContainsKey(newStateId))
        {
            GameLogger.LogWarning(LogCategory.Staff, $"State {newStateId} not found");
            return null;
        }

        currentState?.Exit();
        currentState = states[newStateId];
        currentState.Enter();

        return currentState;
    }

    public void AssignTask(IStaffTask task)
    {
        currentTask = task;
        currentTask.ResetPhaseProgress();
        ChangeState(StaffStateId.ExecutingTask);
    }

    /// <summary>이동 완료 시 State가 호출 (비작업 이동용)</summary>
    public void OnMovementCompleted()
    {
        ChangeState(StaffStateId.Idle);
    }

    /// <summary>Task 완료 시 ExecutingTaskState가 호출</summary>
    public void OnTaskCompleted()
    {
        IStaffTask completedTask = currentTask;

        if (completedTask != null)
        {
            App.TaskQueue.CompleteTask(completedTask);
            App.EventBus.Publish(new TaskCompletedEvent(completedTask, staff));
        }

        if (completedTask.Type == TaskType.CollectResource && completedTask is CollectResourceTask collectResourceTask)
        {
            if (states[StaffStateId.CarryingResource] is StaffCarryingResourceState carryingResourceState)
            {
                carryingResourceState.SetResourceType(collectResourceTask.ResourceType);
            }

            ChangeState(StaffStateId.CarryingResource);
        }
    }

    /// <summary>특정 위치로 이동 (작업 없음)</summary>
    internal void BeginMoveToTarget(Vector3 position)
    {
        if (states[StaffStateId.MovingToTarget] is StaffMovingToTargetState movingState)
        {
            movingState.SetTarget(position);
        }
        ChangeState(StaffStateId.MovingToTarget);
    }

    // ── NavMesh 위임 래퍼 (상태 클래스는 controller만 바라봄) ──

    /// <summary>위치 및 회전 설정</summary>
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        => transform.SetPositionAndRotation(position, rotation);

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position) => staff.SetDestination(position);

    /// <summary>이동 정지</summary>
    public void StopMoving() => staff.StopMoving();

    /// <summary>목적지 도착 여부 확인</summary>
    public bool HasReachedDestination() => staff.HasReachedDestination();

    /// <summary>NavMeshAgent 활성화 설정</summary>
    public void EnableNavMeshAgent(bool enable) => staff.EnableNavMeshAgent(enable);

    /// <summary>자원을 들기</summary>
    public void PickUpResource(FacilityResourceType resourceType, int amount)
    {
        carryingResourceType = resourceType;
        carryingAmount = amount;
        GameLogger.Log(LogCategory.Staff, $"{name}: carrying {resourceType} x{amount}");
        // TODO: 운반 비주얼 활성화
    }

    /// <summary>들고 있는 자원 내려놓기 (조리 시설에 전달 후 호출)</summary>
    public void DropResource()
    {
        GameLogger.Log(LogCategory.Staff, $"{name}: delivered {carryingResourceType} x{carryingAmount}");
        carryingResourceType = FacilityResourceType.None;
        carryingAmount = 0;
        // TODO: 운반 비주얼 비활성화
    }

    /// <summary>애니메이션 파라미터 설정</summary>
    public void SetAnimatorBool(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }

    /// <summary>애니메이션 트리거 설정</summary>
    public void SetAnimatorTrigger(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    /// <summary>모든 Prop 비활성화</summary>
    public void DisableAllProps()
    {
        foreach (var prop in props)
        {
            prop.GameObject.SetActive(false);
        }
    }

    /// <summary>특정 Prop 활성화</summary>
    public void EnableProp(StaffPropId propId)
    {
        foreach (var prop in props)
        {
            if (prop.Id == propId)
            {
                prop.GameObject.SetActive(true);
            }
            else
            {
                prop.GameObject.SetActive(false);
            }
        }
    }
}
