using UnityEngine;
using System.Collections.Generic;

public class GameSessionRunner : MonoBehaviour
{
    [SerializeField] private float dayLengthSeconds = 60f;
    [SerializeField] private PhaseId startingPhase = PhaseId.Preparation;

    private SimClock simClock;
    private SimLoop simLoop;
    private PhaseController phaseController;

    private void Awake()
    {
        // Initialize core simulation state
        simClock = new SimClock(dayLengthSeconds);
        simLoop = new SimLoop(simClock);

        // Build phases with simLoop reference so they can toggle ON/OFF
        var phases = new List<IPhaseState>
        {
            new PreparationPhase(simLoop),
            new OpenPhase(simLoop),
            new ClosingPhase(simLoop),
            new UpgradePhase(simLoop)
        };

        // Initialize controller with provided phases and starting phase
        phaseController = new PhaseController(phases, startingPhase);

        // // Register core simulation systems

        // simLoop.AddSystem(new SeatingSystem());
        // simLoop.AddSystem(new CustomerSystem());
    }

    private void Update()
    {
        // Per-frame: let phase do UI/logic, and advance simulation if enabled
        phaseController.Tick(Time.deltaTime);
        simLoop.Update(Time.deltaTime);
    }

    // Optional external control (e.g., from UI)
    public void ChangePhase(PhaseId next)
    {
        phaseController.Change(next);
    }
}
