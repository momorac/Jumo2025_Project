using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주문 받기 작업 (1 Phase)
/// 손님 테이블로 이동 → 주문 접수
/// </summary>
public class TakeOrderTask : StaffTaskBase
{
    public override TaskType Type => TaskType.TakeOrder;

    private readonly Customer customer;
    private readonly Transform seatPosition;
    private readonly OrderData order;

    public TakeOrderTask(Customer customer, Transform seatPosition, OrderData order, int priority = 10)
        : base(priority)
    {
        this.customer = customer;
        this.seatPosition = seatPosition;
        this.order = order;
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: seatPosition,
            duration: 2f,
            animationTrigger: "TakeOrder",
            onExecute: controller =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{controller.name} taking order");
                App.EventBus.Publish(new OrderTakenEvent(customer, order));
            }
        )
    };
}
