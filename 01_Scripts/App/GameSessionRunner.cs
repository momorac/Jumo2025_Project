using UnityEngine;
using System.Collections.Generic;

public class GameSessionRunner : MonoBehaviour
{
    [SerializeField] private float dayLengthSeconds = 60f;
    [SerializeField] private PhaseId startingPhase = PhaseId.Preparation;

    [Header("Application Systems")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlacementController placementController;

    // 코어 시뮬레이션 인스턴스
    private SimClock simClock;
    private SimLoop simLoop;
    private PhaseController phaseController;

    // 서비스 인스턴스는 App에서 관리

    private void Awake()
    {

        // 저장된 데이터 로드 후 App에 등록
        App.InitializeGameData(SaveService.Load());

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
    }

    private void Start()
    {
        // 세션 의존성 초기화
        uiManager.InjectSessionControllers(placementController);
    }

    private void Update()
    {
        // 매 프레임: 페이즈의 UI/로직을 수행하고, 활성화되어 있으면 시뮬레이션을 진행
        phaseController.Tick(Time.deltaTime);
        simLoop.Update(Time.deltaTime);


        if (phaseController.CurrentPhaseID == PhaseId.Open && simClock.IsDayOver())
        {
            ChangePhase(PhaseId.Closing);
        }
    }

    private void AddSimSystems()
    {
        simLoop.AddSystem(new CustomerSpawnSimSystem());
    }

    // 선택적 외부 제어(예: UI에서)
    public void ChangePhase(PhaseId next)
    {
        phaseController.Change(next);
    }

    // 게임 데이터 로컬 저장
    public void SaveGameData()
    {
        SaveService.Save(App.GetSessionGameData());
    }


    private void OnApplicationQuit()
    {
        // 종료 시 최신 경제 잔액 반영 후 저장
        SaveGameData();

    }
}
