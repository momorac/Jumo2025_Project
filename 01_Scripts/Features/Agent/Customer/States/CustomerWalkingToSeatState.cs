using UnityEngine;

/// <summary>
/// Customer 좌석 이동 상태
/// </summary>
public class CustomerWalkingToSeatState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.WalkingToSeat;

    private readonly CustomerController controller;

    public CustomerWalkingToSeatState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.SetAnimation("IsWalking", true);
        controller.SetDestination(controller.AssignedSeat.position);
        Debug.Log($"<color=yellow>{controller.name}: Walking to seat</color>");
    }

    public void Tick(float deltaTime)
    {
        if (controller.HasReachedDestination())
        {
            controller.SitDown();
        }
    }

    public void Exit()
    {
        controller.SetAnimation("IsWalking", false);
    }
}
