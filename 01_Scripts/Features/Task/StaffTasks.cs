using UnityEngine;

/// <summary>
/// Staff 작업 기본 클래스
/// 공통 기능을 구현하고 구체적인 작업은 상속하여 구현
/// </summary>
public abstract class StaffTaskBase : IStaffTask
{
    private static int nextTaskId = 1;

    public int TaskId { get; }
    public abstract TaskType Type { get; }
    public Transform TargetPosition { get; protected set; }
    public int Priority { get; protected set; }
    public float CreatedTime { get; }
    public Customer AssociatedCustomer { get; protected set; }
    public OrderData AssociatedOrder { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public bool IsCancelled { get; protected set; }

    protected StaffTaskBase(Transform targetPosition, int priority = 0)
    {
        TaskId = nextTaskId++;
        TargetPosition = targetPosition;
        Priority = priority;
        CreatedTime = Time.time;
        IsCompleted = false;
        IsCancelled = false;
    }

    public abstract void Execute(Staff staff);

    public virtual void Complete()
    {
        IsCompleted = true;
        Debug.Log($"<color=green>Task {TaskId} ({Type}) completed</color>");
    }

    public virtual void Cancel()
    {
        IsCancelled = true;
        Debug.Log($"<color=yellow>Task {TaskId} ({Type}) cancelled</color>");
    }
}

/// <summary>
/// 주문 받기 작업
/// </summary>
public class TakeOrderTask : StaffTaskBase
{
    public override TaskType Type => TaskType.TakeOrder;

    public TakeOrderTask(Customer customer, Transform seatPosition, OrderData order, int priority = 10)
        : base(seatPosition, priority)
    {
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    public override void Execute(Staff staff)
    {
        Debug.Log($"<color=cyan>Staff {staff.name} taking order from customer</color>");

        // 주문 접수 이벤트 발행
        App.EventBus.Publish(new OrderTakenEvent(AssociatedCustomer, AssociatedOrder));
    }
}

/// <summary>
/// 음료 서빙 작업
/// </summary>
public class ServeDrinkTask : StaffTaskBase
{
    public override TaskType Type => TaskType.ServeDrink;

    public ServeDrinkTask(Customer customer, Transform seatPosition, OrderData order, int priority = 8)
        : base(seatPosition, priority)
    {
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    public override void Execute(Staff staff)
    {
        Debug.Log($"<color=cyan>Staff {staff.name} serving drink to customer</color>");

        // 서빙 완료 이벤트 발행
        App.EventBus.Publish(new OrderServedEvent(AssociatedCustomer, AssociatedOrder));
    }
}

/// <summary>
/// 음식 서빙 작업
/// </summary>
public class ServeFoodTask : StaffTaskBase
{
    public override TaskType Type => TaskType.ServeFood;

    public ServeFoodTask(Customer customer, Transform seatPosition, OrderData order, int priority = 8)
        : base(seatPosition, priority)
    {
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    public override void Execute(Staff staff)
    {
        Debug.Log($"<color=cyan>Staff {staff.name} serving food to customer</color>");

        // 서빙 완료 이벤트 발행
        App.EventBus.Publish(new OrderServedEvent(AssociatedCustomer, AssociatedOrder));
    }
}

/// <summary>
/// 테이블 청소 작업
/// </summary>
public class CleanTableTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CleanTable;

    public CleanTableTask(Transform tablePosition, int priority = 5)
        : base(tablePosition, priority)
    {
    }

    public override void Execute(Staff staff)
    {
        Debug.Log($"<color=cyan>Staff {staff.name} cleaning table</color>");
    }
}

/// <summary>
/// 계산 작업
/// </summary>
public class CheckoutTask : StaffTaskBase
{
    public override TaskType Type => TaskType.Checkout;

    public CheckoutTask(Customer customer, Transform position, OrderData order, int priority = 6)
        : base(position, priority)
    {
        AssociatedCustomer = customer;
        AssociatedOrder = order;
    }

    public override void Execute(Staff staff)
    {
        Debug.Log($"<color=cyan>Staff {staff.name} processing checkout</color>");

        // 경제 시스템에 수익 추가
        if (AssociatedOrder != null)
        {
            App.EconomyService.AddIncome(AssociatedOrder.Price);
        }
    }
}

/// <summary>
/// 자원 수집 작업
/// Staff가 자원 시설(우물/장작더미)로 이동 → 수집 모션 → 자원을 들고 있는 상태
/// </summary>
public class CollectResourceTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CollectResource;

    public ResourceFacilityBase SourceFacility { get; }
    public FacilityResourceType ResourceType { get; }
    public float CollectDuration { get; }

    public CollectResourceTask(ResourceFacilityBase facility, float collectDuration, int priority = 7)
        : base(facility.transform, priority)
    {
        SourceFacility = facility;
        ResourceType = facility.ProvidedResourceType;
        CollectDuration = collectDuration;
    }

    public override void Execute(Staff staff)
    {
        // 시설에서 자원 수집
        int amount = SourceFacility.CollectResource();

        // Staff에게 자원 할당
        staff.PickUpResource(ResourceType, amount);

        Debug.Log($"<color=cyan>Staff {staff.name}: {ResourceType} {amount}개 수집 완료</color>");
    }
}
