public class ClosingPhase : IPhaseState
{
    public PhaseId Id => PhaseId.Closing;

    private SimLoop simLoop;

    public ClosingPhase(SimLoop simLoop)
    {
        this.simLoop = simLoop;
    }

    public void Enter()
    {
        simLoop.SetEnabled(false);
    }

    public void Tick(float deltaTime)
    {
        // Implement logic for Closing Phase per tick
    }

    public void Exit()
    {
        // Cleanup or transition logic if needed
    }
}
