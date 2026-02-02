using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameSessionRunner : MonoBehaviour
{
    [Header("Game Session Settings")]
    [SerializeField] private float dayLengthSeconds = 60f;
    [SerializeField] private PhaseId startingPhase = PhaseId.Preparation;

    [Header("Application Systems")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlacementController placementController;

    // 코어 시뮬레이션 인스턴스
    private SimClock simClock;
    private SimLoop simLoop;
    private PhaseController phaseController;

    private bool hasInitialized = false;

    private void Awake()
    {
        StartCoroutine(InitializeGameSessionCoroutine());
    }

    private void Update()
    {
        if (!hasInitialized)
            return;

        // 매 프레임: 페이즈의 UI/로직을 수행하고, 활성화되어 있으면 시뮬레이션을 진행
        phaseController.Tick(Time.deltaTime);
        simLoop.Update(Time.deltaTime);

        // 페이즈 전환 조건 확인
        if (phaseController.CurrentPhaseID == PhaseId.Open && simClock.IsDayOver())
        {
            ChangePhase(PhaseId.Closing);
        }
    }


    private IEnumerator InitializeGameSessionCoroutine()
    {
        // 저장된 데이터 로드 후 App에 등록
        App.InitializeGameData(SaveManager.Load());
        yield return new WaitUntil(() => App.HasInitialized);

        // 코어 시뮬레이션 상태 초기화
        simClock = new SimClock(dayLengthSeconds);
        simLoop = new SimLoop(simClock);
        AddSimSystems();

        var phases = new List<IPhaseState>
        {
            new PreparationPhase(simLoop),
            new OpenPhase(simLoop),
            new ClosingPhase(simLoop),
            new UpgradePhase(simLoop)
        };

        phaseController = new PhaseController(phases, startingPhase);

        placementController.Initialize();
        uiManager.InjectSessionControllers(placementController);

        hasInitialized = true;
    }

    // 시뮬레이션 시스템 일괄 추가 메서드
    private void AddSimSystems()
    {
        simLoop.AddSystem(new CustomerSpawnSimSystem());
        simLoop.AddSystem(new PedestrianSpawnSimSystem());
    }

    // 선택적 페이즈 전환 메서드
    public void ChangePhase(PhaseId next)
    {
        phaseController.Change(next);
    }

    // 게임 데이터 로컬 저장
    public void SaveGameData()
    {
        SaveManager.Save(App.GetSessionDataToMeta());
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
