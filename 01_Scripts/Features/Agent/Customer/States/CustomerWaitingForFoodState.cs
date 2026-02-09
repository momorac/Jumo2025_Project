using UnityEngine;

/// <summary>
/// Customer 음식/음료 대기 상태
/// </summary>
public class CustomerWaitingForFoodState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.WaitingForFood;

    private readonly CustomerController controller;
    private float waitTime;
    private float maxWaitTime = 60f;

    public CustomerWaitingForFoodState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        waitTime = 0f;

        // 서빙 완료 이벤트 구독
        App.EventBus.Subscribe<OrderServedEvent>(OnOrderServed);

        Debug.Log($"<color=yellow>{controller.name}: Waiting for food/drink</color>");
    }

    public void Tick(float deltaTime)
    {
        waitTime += deltaTime;

        // 최대 대기 시간 초과 시 이탈
        if (waitTime >= maxWaitTime)
        {
            controller.LeaveWithoutOrder();
        }
    }

    public void Exit()
    {
        App.EventBus.Unsubscribe<OrderServedEvent>(OnOrderServed);
    }

    private void OnOrderServed(OrderServedEvent evt)
    {
        if (evt.Customer == controller.Customer)
        {
            controller.ChangeState(CustomerStateId.Eating);
        }
    }
}
