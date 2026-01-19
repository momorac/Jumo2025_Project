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

    // 서비스 인스턴스

    private EconomyService economy;

    private void Awake()
    {
        // 코어 시뮬레이션 상태 초기화
        simClock = new SimClock(dayLengthSeconds);
        simLoop = new SimLoop(simClock);

        var phases = new List<IPhaseState>
        {
            new PreparationPhase(simLoop),
            new OpenPhase(simLoop),
            new ClosingPhase(simLoop),
            new UpgradePhase(simLoop)
        };

        phaseController = new PhaseController(phases, startingPhase);

        // 저장 로드 후 App에 등록(단일 소스)
        App.SetGameData(SaveService.Load());
        economy = new EconomyService(App.GameData.EconomyBalance);

        // placement 관련 이벤트 구독
        placementController.Initialize(OnPlacementUpdated);
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

    // 선택적 외부 제어(예: UI에서)
    public void ChangePhase(PhaseId next)
    {
        phaseController.Change(next);
    }

    // 게임 데이터 로컬 저장
    public void SaveGameData()
    {
        SaveService.Save(App.GameData);
    }

    // PlacementSystem 이벤트 핸들러
    private void OnPlacementUpdated(PlacementRecord[,] records)
    {
        if (App.GameData == null) return;
        App.SetPlacementData(new PlacementData(placementController.GetGridSize(), records));
        Debug.Log("[GameSessionRunner] Placement data updated in GameMetaData.");
    }

    private void OnApplicationQuit()
    {
        // 종료 시 최신 경제 잔액 반영 후 저장
        App.GameData.EconomyBalance = economy.GetMoney();
        SaveService.Save(App.GameData);

        App.SetGameData(null);
    }
}
