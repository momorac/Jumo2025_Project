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
        controller.SetDestination(controller.AssignedSeat.Root.position);
        GameLogger.LogVerbose(LogCategory.Customer, $"{controller.name}: walking to seat");
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
