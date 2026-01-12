public class UpgradePhase : IPhaseState
{
    public PhaseId Id => PhaseId.Upgrade;

    private SimLoop simLoop;

    public UpgradePhase(SimLoop simLoop)
    {
        this.simLoop = simLoop;
    }

    public void Enter()
    {
        simLoop.SetEnabled(false);
    }

    public void Tick(float deltaTime)
    {
        // Implement logic for Upgrade Phase per tick
    }

    public void Exit()
    {
        // Cleanup or transition logic if needed
    }
}
