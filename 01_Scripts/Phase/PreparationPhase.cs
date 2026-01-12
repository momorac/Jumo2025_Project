
public class PreparationPhase : IPhaseState
{
    public PhaseId Id => PhaseId.Preparation;

    private SimLoop simLoop;

    public PreparationPhase(SimLoop simLoop)
    {
        this.simLoop = simLoop;
    }

    public void Enter()
    {
        simLoop.SetEnabled(false);
    }

    public void Tick(float deltaTime)
    {
        // Implement logic for Preparation Phase per tick
    }

    public void Exit()
    {
        // Cleanup or transition logic if needed
    }
}
