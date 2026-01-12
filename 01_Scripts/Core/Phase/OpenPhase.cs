
public class OpenPhase : IPhaseState
{
    public PhaseId Id => PhaseId.Open;

    private SimLoop simLoop;

    public OpenPhase(SimLoop simLoop)
    {
        this.simLoop = simLoop;
    }

    public void Enter()
    {
        simLoop.SetEnabled(true);
    }

    public void Tick(float deltaTime)
    {
        // Implement logic for Open Phase per tick
    }

    public void Exit()
    {
        simLoop.SetEnabled(false);
    }
}
