
[System.Serializable]
public class OrderData
{
    /// <summary>주문 ID</summary>  
    public int OrderId;

    /// <summary>메뉴 타입</summary>
    public MenuType MenuType;

    /// <summary>메뉴 이름</summary>
    /// </summary>
    public string MenuName;

    /// <summary>가격</summary>
    public int Price;

    /// <summary>조리/준비 시간 (초)</summary>
    public float PrepareTime;

    /// <summary>식사 시간 (초)</summary>
    public float EatTime;

    /// <summary>주문 상태</summary>
    public OrderStatus Status;

    public OrderData()
    {
        OrderId = 0;
        MenuType = MenuType.Drink;
        MenuName = "Unknown";
        Price = 0;
        PrepareTime = 3f;
        EatTime = 5f;
        Status = OrderStatus.Pending;
    }

    public OrderData(int orderId, MenuType menuType, string menuName, int price, float prepareTime = 3f, float eatTime = 5f)
    {
        OrderId = orderId;
        MenuType = menuType;
        MenuName = menuName;
        Price = price;
        PrepareTime = prepareTime;
        EatTime = eatTime;
        Status = OrderStatus.Pending;
    }
}

public enum MenuType
{
    Drink,      // 음료
    Food,       // 음식
    Dessert     // 디저트
}

public enum OrderStatus
{
    Pending,        // 대기 중 (주문 전)
    Ordered,        // 주문됨 (Staff가 주문을 받음)
    Preparing,      // 준비 중
    Ready,          // 준비 완료
    Served,         // 서빙됨
    Completed,      // 완료 (식사 완료)
    Cancelled       // 취소됨
}
