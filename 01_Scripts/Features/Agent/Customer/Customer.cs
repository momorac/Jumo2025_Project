using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Customer 에이전트 — 물리/이동 제어 + 외부 퍼블릭 파사드.
/// NavMeshAgent 등 물리 컴포넌트를 직접 소유.
/// FSM·데이터는 CustomerController에 위임
/// </summary>
[RequireComponent(typeof(CustomerController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour, IPooled
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 0.3f;

    private CustomerController controller;

    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;

    // 현재 주문 접근자
    public OrderData CurrentOrder => controller?.CurrentOrder;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        controller = GetComponent<CustomerController>();
    }

    public void OnGet()
    {
        EnableNavMeshAgent(true);

        GameLogger.Log(LogCategory.Customer, $"{name} spawned");

        // 위치 랜덤 설정
        int randomIdx = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIdx].position;
    }

    public void OnRelease()
    {
        GameLogger.LogVerbose(LogCategory.Customer, $"{name} released to pool");
    }

    public void Release()
    {
        App.PoolService.customerPool.Release(this);
    }

    /// <summary>좌석 배정 (지연 포함) </summary>
    public void SetSeatDealy(Seat seat, float delay)
    {
        StartCoroutine(AssignSeatDelayedCoroutine(seat, delay));
    }

    private IEnumerator AssignSeatDelayedCoroutine(Seat seat, float delay)
    {
        yield return new WaitForSeconds(delay);

        // CustomerController를 통해 FSM 시작
        controller?.AssignSeat(seat);
    }

    /// <summary> 주문 접수됨 </summary>
    public void OnOrderTaken()
    {
        controller?.OnOrderTaken();
    }

    /// <summary> 정상 퇴장 </summary>
    public void Leave()
    {
        controller?.Leave();
    }

    // ── NavMesh 이동 제어 (CustomerController가 위임 래퍼를 통해 호출) ──

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

        return !agent.pathPending && agent.remainingDistance <= stoppingDistance;
    }

    /// <summary>NavMeshAgent 활성화 설정</summary>
    public void EnableNavMeshAgent(bool enable)
    {
        if (agent == null) return;

        if (agent.enabled)
            agent.ResetPath();

        agent.enabled = enable;
    }
}

