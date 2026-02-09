using UnityEngine;

/// <summary>
/// Customer 계산 대기 상태
/// </summary>
public class CustomerWaitingForCheckoutState : ICustomerState
{
    public CustomerStateId Id => CustomerStateId.WaitingForCheckout;

    private readonly CustomerController controller;
    private float waitTime;
    private float maxWaitTime = 30f;

    public CustomerWaitingForCheckoutState(CustomerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        waitTime = 0f;

        // 말풍선 표시 (계산 아이콘)
        controller.ShowBubble(true);

        Debug.Log($"<color=yellow>{controller.name}: Waiting for checkout</color>");
    }

    public void Tick(float deltaTime)
    {
        waitTime += deltaTime;

        // 일단은 자동으로 일정 시간 후 퇴장 (나중에 Checkout Task로 연동)
        if (waitTime >= 3f)
        {
            controller.Leave();
        }
    }

    public void Exit()
    {
        controller.ShowBubble(false);
    }
}
