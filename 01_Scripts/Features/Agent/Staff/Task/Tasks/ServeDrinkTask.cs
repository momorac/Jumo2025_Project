using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 음료 서빙 작업 (1 Phase)
/// 손님 테이블로 이동 → 음료 서빙
/// </summary>
public class ServeDrinkTask : StaffTaskBase
{
    public override TaskType Type => TaskType.ServeDrink;

    private readonly Customer customer;
    private readonly Transform seatPosition;
    private readonly OrderData order;

    public ServeDrinkTask(Customer customer, Transform seatPosition, OrderData order, int priority = 8)
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
            duration: 1.5f,
            onStart: staff => staff.SetAnimatorTrigger("ServeDrink"),
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} serving drink");
                App.EventBus.Publish(new OrderServedEvent(customer, order));
            }
        )
    };
}
