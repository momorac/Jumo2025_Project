using UnityEngine;
using System.Collections.Generic;

public class GameSessionRunner : MonoBehaviour
{
    [SerializeField] private float dayLengthSeconds = 60f;
    [SerializeField] private PhaseId startingPhase = PhaseId.Preparation;

    [Header("Application Systems")]
    [SerializeField] private PlacementSystem placementSystem;
    public PlacementSystem PlacementSystem => placementSystem;
    [SerializeField] private FacilityPlacementController facilityPlacementController;
    public FacilityPlacementController FacilityPlacementController => facilityPlacementController;

    // Core simulation instances
    private SimClock simClock;
    private SimLoop simLoop;
    private PhaseController phaseController;

    // Service instances
    private EconomyService economy;

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

        // Economy as on-demand service (not auto-ticked)
        var save = SaveService.Load();
        economy = new EconomyService(save.EconomyBalance);
    }

    private void Update()
    {
        // Per-frame: let phase do UI/logic, and advance simulation if enabled
        phaseController.Tick(Time.deltaTime);
        simLoop.Update(Time.deltaTime);


        if (phaseController.CurrentPhaseID == PhaseId.Open && simClock.IsDayOver())
        {
            ChangePhase(PhaseId.Closing);
        }
    }

    // Optional external control (e.g., from UI)
    public void ChangePhase(PhaseId next)
    {
        phaseController.Change(next);
    }

    private void OnApplicationQuit()
    {
        SaveService.Save(new MetaGameData { EconomyBalance = economy.GetMoney() });
    }
}
