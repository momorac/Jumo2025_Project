using UnityEngine;

/// <summary>
/// Customer 주문 대기 상태
/// 말풍선 표시, 일정 시간 후 이탈 가능
/// </summary>
public class CustomerWaitingToOrderState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.WaitingToOrder;

    private readonly CustomerController controller;
    private float waitTime;
    private float maxWaitTime = 30f; // 최대 대기 시간

    public CustomerWaitingToOrderState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        waitTime = 0f;

        // 말풍선 표시
        controller.ShowBubble(true);

        // 주문 준비 이벤트 발행
        App.EventBus.Publish(new CustomerReadyToOrderEvent(
            controller.Customer,
            controller.AssignedSeat,
            controller.CurrentOrder
        ));

        Debug.Log($"<color=yellow>{controller.name}: Waiting to order (bubble shown)</color>");
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
        controller.ShowBubble(false);
    }

    /// <summary>
    /// 최대 대기 시간 설정
    /// </summary>
    public void SetMaxWaitTime(float time)
    {
        maxWaitTime = time;
    }
}
