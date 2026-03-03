using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Staff 작업 기본 클래스
/// 공통 기능을 구현하고 구체적인 작업은 상속하여 구현.
/// 각 Task는 BuildPhases()를 오버라이드하여 실행 단계를 정의.
/// </summary>
public abstract class StaffTaskBase : IStaffTask
{
    private static int nextTaskId = 1;

    public int TaskId { get; }
    public abstract TaskType Type { get; }
    public int Priority { get; protected set; }
    public float CreatedTime { get; }
    public Customer AssociatedCustomer { get; protected set; }
    public OrderData AssociatedOrder { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public bool IsCancelled { get; protected set; }

    // Phases 지연 초기화 (base 생성자 → 하위 필드 미초기화 문제 방지)
    private IReadOnlyList<TaskPhase> _phases;
    public IReadOnlyList<TaskPhase> Phases => _phases ??= BuildPhases();

    protected StaffTaskBase(int priority = 0)
    {
        TaskId = nextTaskId++;
        Priority = priority;
        CreatedTime = Time.time;
        IsCompleted = false;
        IsCancelled = false;
    }

    /// <summary>작업의 실행 단계를 정의. 하위 클래스에서 오버라이드</summary>
    protected abstract List<TaskPhase> BuildPhases();

    public virtual void Complete()
    {
        IsCompleted = true;
        GameLogger.Log(LogCategory.Task, $"Task {TaskId} ({Type}) completed");
    }

    public virtual void Cancel()
    {
        IsCancelled = true;
        GameLogger.LogWarning(LogCategory.Task, $"Task {TaskId} ({Type}) cancelled");
    }
}

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
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} taking order");
                App.EventBus.Publish(new OrderTakenEvent(customer, order));
            },
            onStart: staff => staff.Controller.SetAnimatorTrigger("TakeOrder")
        )
    };
}

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
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} serving drink");
                App.EventBus.Publish(new OrderServedEvent(customer, order));
            },
            onStart: staff => staff.Controller.SetAnimatorTrigger("ServeDrink")
        )
    };
}

/// <summary>
/// 음식 서빙 작업 (1 Phase)
/// 손님 테이블로 이동 → 음식 서빙
/// </summary>
public class ServeFoodTask : StaffTaskBase
{
    public override TaskType Type => TaskType.ServeFood;

    private readonly Customer customer;
    private readonly Transform seatPosition;
    private readonly OrderData order;

    public ServeFoodTask(Customer customer, Transform seatPosition, OrderData order, int priority = 8)
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
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} serving food");
                App.EventBus.Publish(new OrderServedEvent(customer, order));
            },
            onStart: staff => staff.Controller.SetAnimatorTrigger("ServeFood")
        )
    };
}

/// <summary>
/// 테이블 청소 작업 (1 Phase)
/// 테이블로 이동 → 청소
/// </summary>
public class CleanTableTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CleanTable;

    private readonly Transform tablePosition;

    public CleanTableTask(Transform tablePosition, int priority = 5)
        : base(priority)
    {
        this.tablePosition = tablePosition;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: tablePosition,
            duration: 3f,
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} cleaning table");
            },
            onStart: staff => staff.Controller.SetAnimatorTrigger("CleanTable")
        )
    };
}

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
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} processing checkout");
                if (order != null)
                {
                    App.EconomyService.AddIncome(order.Price);
                }
            },
            onStart: staff => staff.Controller.SetAnimatorTrigger("Checkout")
        )
    };
}

/// <summary>
/// 자원 수집 작업 (3 Phase)
/// 자원 시설(우물/장작더미)로 이동 → 수집 모션 → 자원을 들고 있는 상태
/// </summary>
public class CollectResourceTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CollectResource;

    public ResourceFacilityBase SourceFacility { get; }
    public FacilityResourceType ResourceType { get; }

    public CollectResourceTask(ResourceFacilityBase facility, float collectDuration, int priority = 7)
        : base(priority)
    {
        SourceFacility = facility;
        ResourceType = facility.ProvidedResourceType;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: SourceFacility.transform,
            duration: SourceFacility.CollectDuration,
            onStart: (staff) =>
            {
                staff.Controller.SetCharacterDirection(SourceFacility.transform);
                if (SourceFacility.FacilityType == FacilityType.Stump)
                {
                    staff.Controller.ActivateProp(StaffPropId.Axe);
                }
                staff.Controller.SetAnimatorBool("IsWorking", true);
                staff.Controller.SetAnimatorTrigger(SourceFacility.FacilityType == FacilityType.Well ? "CollectWater" : "CollectFirewood");
            },
            onExecute: (staff) =>
            {
                int amount = SourceFacility.CollectResource();
                staff.PickUpResource(ResourceType, amount);
                GameLogger.Log(LogCategory.Task, $"{staff.name}: {ResourceType} x{amount} collected");
            },
            onEnd: (staff) =>
            {
                staff.Controller.SetAnimatorBool("IsWorking", false);
                staff.Controller.DeactivateAllProps();
            }
        )
    };
}
