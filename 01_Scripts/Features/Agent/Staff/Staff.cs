using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Staff 에이전트 — 물리/이동 제어 + 외부 퍼블릭 파사드
/// NavMeshAgent 등 물리 컴포넌트를 직접 소유
/// FSM·데이터는 StaffController에 위임
/// </summary>
[RequireComponent(typeof(StaffController))]
public class Staff : MonoBehaviour, IClickable
{
    [Header("Settings")]
    [SerializeField] private int clickPriority = 5;
    [SerializeField] private float rotationSpeed = 720f; // 회전 속도 (도/초)
    [SerializeField] private Transform targetTransform;
    public Transform TargetTransform => targetTransform;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;

    private StaffController controller;
    internal StaffController Controller => controller;

    // IClickable 구현
    public bool IsClickable => true;
    public int ClickPriority => clickPriority;

    // 상태 접근자
    public bool IsIdle => controller?.IsIdle ?? false;

    private void Awake()
    {
        controller = GetComponent<StaffController>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.updateRotation = false; // 회전은 애니메이션과 커스텀 로직으로 제어
    }

    private void Update()
    {
        if (agent.pathPending || !agent.hasPath)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance * 2)
        {
            agent.ResetPath();
            return;
        }

        Vector3 dir = agent.desiredVelocity;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    /// <summary>클릭 시 Staff 선택</summary>
    public void OnClicked(Vector3 hitPoint)
    {
        App.StaffRegistry.SelectStaff(this);
        GameLogger.Log(LogCategory.Staff, $"Staff {name} selected");
    }

    /// <summary>작업 배정 (StaffController에 위임)</summary>
    public void AssignTask(IStaffTask task)
    {
        controller?.AssignTask(task);
    }

    /// <summary>특정 위치로 이동 (StaffController에 위임)</summary>
    public void MoveTo(Vector3 position)
    {
        controller?.BeginMoveToTarget(position);
    }

    // ── NavMesh 이동 제어 (StaffController가 위임 래퍼를 통해 호출) ──

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position)
    {
        if (agent == null || !agent.enabled)
            return;

        agent.SetDestination(position);
    }

    /// <summary>이동 정지</summary>
    public void StopMoving()
    {
        if (agent != null && agent.enabled)
            agent.ResetPath();
    }

    /// <summary>목적지 도착 여부 확인</summary>
    public bool HasReachedDestination()
    {
        if (agent == null || !agent.enabled)
            return true;

        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    /// <summary>NavMeshAgent 활성화 설정</summary>
    public void EnableNavMeshAgent(bool enable)
    {
        if (agent == null) return;

        if (agent.enabled)
            agent.ResetPath();

        agent.enabled = enable;
    }


    /// <summary>위치 및 회전 설정</summary>
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }
}
