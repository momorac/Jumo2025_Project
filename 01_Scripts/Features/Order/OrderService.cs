using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주문 관리 서비스
/// 주문 생성, 상태 관리, 완료 처리
/// </summary>
public class OrderService
{
    private static int nextOrderId = 1;
    private readonly Dictionary<int, OrderData> activeOrders = new();
    private readonly List<MenuData> availableMenus = new();

    public event Action<OrderData> OnOrderCreated;
    public event Action<OrderData> OnOrderStatusChanged;
    public event Action<OrderData> OnOrderCompleted;

    public OrderService()
    {
        // 기본 메뉴 초기화 (나중에 ScriptableObject로 교체 가능)
        InitializeDefaultMenus();
    }

    private void InitializeDefaultMenus()
    {
        availableMenus.Add(new MenuData("아메리카노", MenuType.Drink, 4500, 2f, 3f));
        availableMenus.Add(new MenuData("카페라떼", MenuType.Drink, 5000, 3f, 4f));
        availableMenus.Add(new MenuData("샌드위치", MenuType.Food, 7000, 5f, 8f));
        availableMenus.Add(new MenuData("케이크", MenuType.Dessert, 6000, 2f, 5f));
    }

    /// <summary>
    /// 랜덤 메뉴로 주문 생성
    /// </summary>
    public OrderData CreateRandomOrder()
    {
        if (availableMenus.Count == 0)
        {
            Debug.LogWarning("No menus available!");
            return new OrderData();
        }

        var randomMenu = availableMenus[UnityEngine.Random.Range(0, availableMenus.Count)];

        var order = new OrderData(
            nextOrderId++,
            randomMenu.Type,
            randomMenu.Name,
            randomMenu.Price,
            randomMenu.PrepareTime,
            randomMenu.EatTime
        );

        activeOrders[order.OrderId] = order;
        OnOrderCreated?.Invoke(order);

        Debug.Log($"<color=blue>Order created: {order.MenuName} (${order.Price})</color>");
        return order;
    }

    /// <summary>
    /// 주문 상태 변경
    /// </summary>
    public void UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        if (activeOrders.TryGetValue(orderId, out var order))
        {
            order.Status = newStatus;
            OnOrderStatusChanged?.Invoke(order);

            Debug.Log($"<color=blue>Order {orderId} status changed to: {newStatus}</color>");

            if (newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled)
            {
                OnOrderCompleted?.Invoke(order);
            }
        }
    }

    /// <summary>
    /// 주문 조회
    /// </summary>
    public OrderData GetOrder(int orderId)
    {
        return activeOrders.TryGetValue(orderId, out var order) ? order : null;
    }

    /// <summary>
    /// 활성 주문 목록 조회
    /// </summary>
    public IReadOnlyDictionary<int, OrderData> GetActiveOrders() => activeOrders;

    /// <summary>
    /// 주문 완료 처리 및 수익 반영
    /// </summary>
    public void CompleteOrder(int orderId)
    {
        if (activeOrders.TryGetValue(orderId, out var order))
        {
            order.Status = OrderStatus.Completed;

            // 경제 시스템에 수익 추가
            App.EconomyService.AddIncome(order.Price);

            OnOrderCompleted?.Invoke(order);
            activeOrders.Remove(orderId);

            Debug.Log($"<color=green>Order {orderId} completed! +${order.Price}</color>");
        }
    }

    /// <summary>
    /// 주문 취소 처리
    /// </summary>
    public void CancelOrder(int orderId)
    {
        if (activeOrders.TryGetValue(orderId, out var order))
        {
            order.Status = OrderStatus.Cancelled;
            OnOrderCompleted?.Invoke(order);
            activeOrders.Remove(orderId);

            Debug.Log($"<color=yellow>Order {orderId} cancelled</color>");
        }
    }
}

/// <summary>
/// 메뉴 정의 데이터
/// </summary>
public class MenuData
{
    public string Name;
    public MenuType Type;
    public int Price;
    public float PrepareTime;
    public float EatTime;

    public MenuData(string name, MenuType type, int price, float prepareTime, float eatTime)
    {
        Name = name;
        Type = type;
        Price = price;
        PrepareTime = prepareTime;
        EatTime = eatTime;
    }
}
