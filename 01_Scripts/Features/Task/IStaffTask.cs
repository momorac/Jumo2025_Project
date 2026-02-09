using UnityEngine;

/// <summary>
/// Staff 작업 타입 정의
/// </summary>
public enum TaskType
{
    TakeOrder,      // 주문 받기
    ServeDrink,     // 음료 서빙
    ServeFood,      // 음식 서빙
    CleanTable,     // 테이블 청소
    Checkout        // 계산
}

/// <summary>
/// Staff 작업 인터페이스
/// 각 작업은 이 인터페이스를 구현하여 TaskQueue에서 관리됨
/// </summary>
public interface IStaffTask
{
    /// <summary> 작업 고유 ID </summary>
    int TaskId { get; }

    /// <summary> 작업 타입 </summary>
    TaskType Type { get; }

    /// <summary> 작업 대상 위치 </summary>
    Transform TargetPosition { get; }

    /// <summary> 작업 우선순위 (높을수록 먼저 처리) </summary>
    int Priority { get; }

    /// <summary> 작업 생성 시간 (FIFO 정렬용) </summary>
    float CreatedTime { get; }

    /// <summary> 연관된 Customer (없으면 null) </summary>
    Customer AssociatedCustomer { get; }

    /// <summary> 연관된 주문 데이터 (없으면 null) </summary>
    OrderData AssociatedOrder { get; }

    /// <summary> 작업 실행 (Staff가 도착 후 호출) </summary>
    /// <param name="staff">작업을 수행하는 Staff</param>
    void Execute(Staff staff);

    /// <summary> 작업 완료 처리 </summary>
    void Complete();

    /// <summary> 작업 취소 처리 </summary>
    void Cancel();

    /// <summary> 작업 완료 여부 </summary>
    bool IsCompleted { get; }

    /// <summary> 작업 취소 여부 </summary>
    bool IsCancelled { get; }
}
