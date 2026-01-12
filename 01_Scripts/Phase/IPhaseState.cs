public enum PhaseId { Preparation, Open, Closing, Upgrade }

public interface IPhaseState
{
    PhaseId Id { get; }
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
