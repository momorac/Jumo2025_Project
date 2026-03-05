using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance;

    [Header("Props")]
    [SerializeField] private List<StaffProp> props;


    // FSM
    private Dictionary<StaffStateId, IStaffState> states;
    private IStaffState currentState;

    // 참조
    private Staff staff;
    private IStaffTask currentTask;
    private int currentPhaseIndex;

    // MovingState 참조 (목표 설정용)
    private StaffMovingToTargetState movingState;

    public Staff Staff => staff;
    public IStaffTask CurrentTask => currentTask;
    public int CurrentPhaseIndex => currentPhaseIndex;
    public TaskPhase CurrentPhase => currentTask?.Phases != null && currentPhaseIndex < currentTask.Phases.Count
        ? currentTask.Phases[currentPhaseIndex] : null;
    public StaffStateId CurrentStateId => currentState?.Id ?? StaffStateId.Idle;
    public bool IsIdle => CurrentStateId == StaffStateId.Idle;

    private void Awake()
    {
        staff = GetComponent<Staff>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

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
        DeactivateAllProps();
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
        movingState = new StaffMovingToTargetState(this);

        states = new Dictionary<StaffStateId, IStaffState>
        {
            { StaffStateId.Idle, new StaffIdleState(this) },
            { StaffStateId.MovingToTarget, movingState },
            { StaffStateId.ExecutingTask, new StaffExecutingTaskState(this) }
        };
    }

    public void ChangeState(StaffStateId newStateId)
    {
        if (!states.ContainsKey(newStateId))
        {
            GameLogger.LogWarning(LogCategory.Staff, $"State {newStateId} not found");
            return;
        }

        currentState?.Exit();
        currentState = states[newStateId];
        currentState.Enter();
    }

    public void AssignTask(IStaffTask task)
    {
        currentTask = task;
        currentPhaseIndex = 0;
        ExecuteCurrentPhase();
    }

    /// <summary>현재 Phase의 이동/실행 시작</summary>
    private void ExecuteCurrentPhase()
    {
        var phase = CurrentPhase;
        if (phase == null)
        {
            // Phase가 없으면 즉시 완료
            CompleteCurrentTask();
            return;
        }

        GameLogger.LogVerbose(LogCategory.Staff,
            $"{name}: Phase {currentPhaseIndex + 1}/{currentTask.Phases.Count} of {currentTask.Type}");

        if (phase.WillMoveFirst)
        {
            movingState.SetTarget(phase.MoveTarget.position, withTask: true);
            ChangeState(StaffStateId.MovingToTarget);
        }
        else
        {
            // 이동 없이 바로 실행
            ChangeState(StaffStateId.ExecutingTask);
        }
    }

    /// <summary>현재 Phase 완료 시 호출. 다음 Phase 또는 Task 완료 처리</summary>
    public void OnPhaseCompleted()
    {
        // 현재 Phase 애니메이션 정리
        StopPhaseAnimation();

        currentPhaseIndex++;

        if (currentTask != null && currentPhaseIndex < currentTask.Phases.Count)
        {
            // 다음 Phase 실행
            ExecuteCurrentPhase();
        }
        else
        {
            // 모든 Phase 완료
            CompleteCurrentTask();
        }
    }

    /// <summary>Task의 모든 Phase 완료 처리</summary>
    private void CompleteCurrentTask()
    {
        var task = currentTask;
        if (task != null)
        {
            App.TaskQueue.CompleteTask(task);
            App.EventBus.Publish(new TaskCompletedEvent(task, staff));
        }

        currentTask = null;
        currentPhaseIndex = 0;
        ChangeState(StaffStateId.Idle);
    }

    /// <summary>NavMeshAgent 활성화 설정</summary>
    public void EnableNavMeshAgent(bool enable)
    {
        if (agent != null)
        {
            if (agent.enabled)
            {
                agent.ResetPath();
            }
            agent.enabled = enable;
        }
    }

    /// <summary> 특정 위치로 이동 (작업 없음)</summary>
    public void MoveTo(Vector3 position)
    {
        movingState.SetTarget(position, withTask: false);
        ChangeState(StaffStateId.MovingToTarget);
    }

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position)
    {
        agent.updateRotation = true;
        agent.SetDestination(position);
    }

    /// <summary>이동 정지</summary>
    public void StopMoving()
    {
        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
        }
    }

    /// <summary>목적지 도착 여부 확인</summary>
    public bool HasReachedDestination()
    {
        if (agent == null || !agent.enabled)
            return true;

        return !agent.pathPending && agent.remainingDistance <= stoppingDistance;
    }


    /// <summary>캐릭터 위치 및 방향 설정</summary>
    public void SetCharacterPositionAndRotation(Vector3 vector3, Quaternion rotation)
    {
        transform.SetPositionAndRotation(vector3, rotation);
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

    /// <summary>Phase 실행 애니메이션 재생 (도착 후 Executing 진입 시 호출)</summary>
    public void PlayPhaseAnimation(TaskPhase phase)
    {
        SetAnimatorBool("IsWorking", true);
    }

    /// <summary>Phase 애니메이션 정리</summary>
    private void StopPhaseAnimation()
    {
        SetAnimatorBool("IsWorking", false);
    }

    /// <summary>모든 Prop 비활성화</summary>
    public void DeactivateAllProps()
    {
        foreach (var prop in props)
        {
            prop.GameObject.SetActive(false);
        }
    }

    /// <summary>특정 Prop 활성화</summary>
    public void ActivateProp(StaffPropId propId)
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
