using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Customer 에이전트 / CustomerController를 통해 FSM 관리
/// </summary>
[RequireComponent(typeof(CustomerController))]
public class Customer : MonoBehaviour, IPooled
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;
    private CustomerController controller;

    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;
    private bool hasInitialized = false;

    // 현재 주문 접근자
    public OrderData CurrentOrder => controller?.CurrentOrder;

    public void OnGet()
    {
        if (!hasInitialized)
        {
            agent = GetComponent<NavMeshAgent>();
            controller = GetComponent<CustomerController>();
            controller.Initialize(this, agent, animator);

            hasInitialized = true;
        }

        if (agent == null || animator == null)
        {
            GameLogger.LogWarning(LogCategory.Customer, $"{nameof(Customer)}: NavMeshAgent or Animator missing");
            return;
        }

        // NavMeshAgent 활성화
        agent.enabled = true;

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

    /// <summary>
    /// 좌석 배정 (지연 포함)
    /// </summary>
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
}

