using System.Collections.Generic;
using UnityEngine;

public class PhaseController
{
    private readonly Dictionary<PhaseId, IPhaseState> phases = new();

    private IPhaseState currentPhase;
    public PhaseId CurrentPhaseID => currentPhase.Id;


    public PhaseController(IEnumerable<IPhaseState> phaseStates, PhaseId start)
    {
        foreach (var phase in phaseStates)
        {
            phases[phase.Id] = phase;
        }

        Change(start);
    }

    public void Change(PhaseId next)
    {
        Debug.Log($"[PhaseController] Changing phase from {currentPhase?.ToString()} to {next.ToString()}");

        currentPhase?.Exit();
        currentPhase = phases[next];
        currentPhase.Enter();
    }

    public void Tick(float deltaTime)
    {
        currentPhase.Tick(deltaTime);
    }



    // public PhaseController(PhaseId startingPhase)
    // {
    //     PreparationPhase = new PreparationPhase(simLoop);
    //     OpenPhase = new OpenPhase(simLoop);
    //     ClosingPhase = new ClosingPhase(simLoop);
    //     UpgradePhase = new UpgradePhase(simLoop);
    // }

}
