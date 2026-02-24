/// <summary> Customer 상태 ID/// </summary>
public enum CustomerStateId
{
    Spawned,            // 스폰됨
    WalkingToSeat,      // 좌석으로 이동 중
    WaitingToOrder,     // 주문 대기 중 (말풍선 표시)
    WaitingForFood,     // 음식/음료 대기 중
    Eating,             // 식사 중
    WaitingForCheckout, // 계산 대기 중
    Leaving             // 퇴장 중
}

/// <summary>Customer 상태 인터페이스</summary>
public interface ICustomerState
{
    /// <summary>상태 ID</summary>
    CustomerStateId Id { get; }

    /// <summary> 상태 진입 시 호출 </summary>
    void Enter();

    /// <summary> 매 프레임 호출 </summary>
    void Tick(float deltaTime);

    /// <summary> 상태 종료 시 호출 </summary>
    void Exit();
}
