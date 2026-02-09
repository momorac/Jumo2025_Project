using UnityEngine;

/// <summary>
/// Customer 퇴장 상태
/// </summary>
public class CustomerLeavingState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.Leaving;

    private readonly CustomerController controller;
    private Transform exitPoint;

    public CustomerLeavingState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        // 좌석 해제
        controller.ReleaseSeat();

        // 출구로 이동
        exitPoint = GetExitPoint();
        if (exitPoint != null)
        {
            controller.EnableNavMeshAgent(true);
            controller.SetDestination(exitPoint.position);
            controller.SetAnimation("IsWalking", true);
        }

        // 퇴장 이벤트 발행
        App.EventBus.Publish(new CustomerLeftEvent(
            controller.Customer,
            controller.AssignedSeat,
            controller.WasServed
        ));

        Debug.Log($"<color=yellow>{controller.name}: Leaving</color>");
    }

    public void Tick(float deltaTime)
    {
        if (controller.HasReachedDestination())
        {
            // 풀에 반환
            controller.ReturnToPool();
        }
    }

    public void Exit()
    {
        controller.SetAnimation("IsWalking", false);
    }

    private Transform GetExitPoint()
    {
        var spawnPoints = App.Anchors?.CustomerSpawnPoints;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        return null;
    }
}
