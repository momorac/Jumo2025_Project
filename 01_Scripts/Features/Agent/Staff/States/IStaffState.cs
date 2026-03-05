/// <summary>
/// Staff 상태 ID
/// </summary>
public enum StaffStateId
{
    Idle,               // 대기 상태
    MovingToTarget,     // 목표 위치로 이동 중
    ExecutingTask,       // 작업 수행 중
    CarryingResource    // 자원 운반 중
}

/// <summary>
/// Staff 상태 인터페이스
/// Phase FSM 패턴과 동일한 구조
/// </summary>
public interface IStaffState
{
    StaffStateId Id { get; }
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
