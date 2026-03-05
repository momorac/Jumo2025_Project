using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 계산 작업 (1 Phase)
/// 계산대로 이동 → 계산 처리
/// </summary>
public class CheckoutTask : StaffTaskBase
{
    public override TaskType Type => TaskType.Checkout;

    private readonly Customer customer;
    private readonly Transform position;
    private readonly OrderData order;

    public CheckoutTask(Customer customer, Transform position, OrderData order, int priority = 6)
        : base(priority)
    {
        this.customer = customer;
        this.position = position;
        this.order = order;
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: position,
            duration: 2f,
            animationTrigger: "Checkout",
            onExecute: controller =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{controller.name} processing checkout");
                if (order != null)
                {
                    App.EconomyService.AddIncome(order.Price);
                }
            }
        )
    };
}
