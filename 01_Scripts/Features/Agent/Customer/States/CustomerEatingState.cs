using UnityEngine;

/// <summary>
/// Customer 식사 상태
/// </summary>
public class CustomerEatingState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.Eating;

    private readonly CustomerController controller;
    private float eatTime;
    private float elapsedTime;

    public CustomerEatingState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        elapsedTime = 0f;
        eatTime = controller.CurrentOrder?.EatTime ?? 5f;

        controller.SetAnimation("IsEating", true);
        GameLogger.LogVerbose(LogCategory.Customer, $"{controller.name}: eating ({eatTime}s)");
    }

    public void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;

        if (elapsedTime >= eatTime)
        {
            // 식사 완료 → 계산 대기
            controller.ChangeState(CustomerStateId.WaitingForCheckout);
        }
    }

    public void Exit()
    {
        controller.SetAnimation("IsEating", false);
    }
}
