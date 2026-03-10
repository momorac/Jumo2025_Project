using UnityEngine;

// ===== Customer 관련 이벤트 =====

#region Customer Events
/// <summary> Customer가 좌석에 앉아서 주문할 준비가 된 경우 </summary>
public struct CustomerReadyToOrderEvent : IGameEvent
{
    public Customer Customer;
    public Transform SeatTransform;
    public OrderData Order;

    public CustomerReadyToOrderEvent(Customer customer, Seat seat, OrderData order)
    {
        Customer = customer;
        SeatTransform = seat.Root;
        Order = order;
    }
}

/// <summary> Customer가 주문을 취소하고 떠나는 경우 (대기 시간 초과 등) </summary>
public struct CustomerLeftEvent : IGameEvent
{
    public Customer Customer;
    public Transform SeatTransform;
    public bool WasServed;

    public CustomerLeftEvent(Customer customer, Seat seat, bool wasServed)
    {
        Customer = customer;
        SeatTransform = seat.Root;
        WasServed = wasServed;
    }
}
#endregion

#region Staff Task Events
/// <summary> 새로운 Task가 TaskQueue에 추가된 경우 </summary>
public struct TaskCreatedEvent : IGameEvent
{
    public IStaffTask Task;

    public TaskCreatedEvent(IStaffTask task)
    {
        Task = task;
    }
}

/// <summary>Task가 Staff에게 배정된 경우</summary>
public struct TaskAssignedEvent : IGameEvent
{
    public IStaffTask Task;
    public Staff Staff;

    public TaskAssignedEvent(IStaffTask task, Staff staff)
    {
        Task = task;
        Staff = staff;
    }
}

/// <summary>Task가 완료된 경우</summary>
public struct TaskCompletedEvent : IGameEvent
{
    public IStaffTask Task;
    public Staff Staff;

    public TaskCompletedEvent(IStaffTask task, Staff staff)
    {
        Task = task;
        Staff = staff;
    }
}

#endregion

#region Order Events

/// <summary>주문이 접수된 경우</summary>
public struct OrderTakenEvent : IGameEvent
{
    public Customer Customer;
    public OrderData Order;

    public OrderTakenEvent(Customer customer, OrderData order)
    {
        Customer = customer;
        Order = order;
    }
}

/// <summary>음식/음료가 서빙된 경우</summary>
public struct OrderServedEvent : IGameEvent
{
    public Customer Customer;
    public OrderData Order;

    public OrderServedEvent(Customer customer, OrderData order)
    {
        Customer = customer;
        Order = order;
    }
}
#endregion

#region Pointing Events

/// <summary>말풍선이 클릭된 경우</summary>
public struct BubbleClickedEvent : IGameEvent
{
    public Customer Customer;
    public Transform TargetPosition;

    public BubbleClickedEvent(Customer customer, Transform targetPosition)
    {
        Customer = customer;
        TargetPosition = targetPosition;
    }
}

/// <summary>Staff가 수동으로 선택된 경우</summary>
public struct StaffSelectedEvent : IGameEvent
{
    public Staff Staff;

    public StaffSelectedEvent(Staff staff)
    {
        Staff = staff;
    }
}

/// <summary>목적지가 클릭된 경우 (Staff 수동 이동)</summary>
public struct DestinationClickedEvent : IGameEvent
{
    public Vector3 Position;
    public IClickable ClickedObject;

    public DestinationClickedEvent(Vector3 position, IClickable clickedObject)
    {
        Position = position;
        ClickedObject = clickedObject;
    }
}
#endregion

#region Facility Events

/// <summary>조리시설 클릭 이벤트</summary>
public struct CookingFacilityClickedEvent : IGameEvent
{
    public ICookingFacility Facility;

    public CookingFacilityClickedEvent(ICookingFacility facility)
    {
        Facility = facility;
    }
}

/// <summary>조리 시설의 자원이 부족한 경우</summary>
public struct FacilityResourceLowEvent : IGameEvent
{
    public ICookingFacility Facility;
    public FacilityResourceType ResourceType;
    public float CurrentRatio;

    public FacilityResourceLowEvent(ICookingFacility facility, FacilityResourceType resourceType, float currentRatio)
    {
        Facility = facility;
        ResourceType = resourceType;
        CurrentRatio = currentRatio;
    }
}

#endregion