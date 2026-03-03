using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float stoppingDistance = 0.5f;

    // FSM
    private Dictionary<StaffStateId, IStaffState> states;
    private IStaffState currentState;

    // 참조
    private Staff staff;
    private IStaffTask currentTask;

    // MovingState 참조 (목표 설정용)
    private StaffMovingToTargetState movingState;

    public Staff Staff => staff;
    public IStaffTask CurrentTask => currentTask;
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

        if (task.TargetPosition != null)
        {
            movingState.SetTarget(task.TargetPosition.position, withTask: true);
            ChangeState(StaffStateId.MovingToTarget);
        }
        else
        {
            // 위치 이동 없이 바로 실행
            ChangeState(StaffStateId.ExecutingTask);
        }
    }

    /// <summary> 특정 위치로 이동 (작업 없음)</summary>
    public void MoveTo(Vector3 position)
    {
        movingState.SetTarget(position, withTask: false);
        ChangeState(StaffStateId.MovingToTarget);
    }

    /// <summary>현재 작업 해제</summary>
    public void ClearCurrentTask()
    {
        currentTask = null;
    }

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position)
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(position);
        }
    }

    /// <summary>이동 정지</summary>
    /// </summary>
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

    /// <summary>애니메이션 파라미터 설정</summary>
    public void SetAnimation(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }

    /// <summary>애니메이션 트리거 설정</summary>
    public void TriggerAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
