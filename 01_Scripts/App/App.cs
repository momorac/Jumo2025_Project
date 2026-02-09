
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class App
{
    // 게임 데이터 도메인
    private static SessionState SessionState;
    private static Economy EconomyData;
    private static PlaceableData PlaceableData;
    private static PlacementData PlacementData;

    // 서비스/매니저
    public static SessionService SessionService { get; private set; }
    public static EconomyService EconomyService { get; private set; }
    public static PlaceableService PlaceableService { get; private set; }
    public static PoolService PoolService { get; private set; }

    // 새로운 서비스들
    public static GameEventBus EventBus { get; private set; }
    public static OrderService OrderService { get; private set; }
    public static TaskQueue TaskQueue { get; private set; }
    public static TaskAssigner TaskAssigner { get; private set; }
    public static StaffRegistry StaffRegistry { get; private set; }

    public static GameAnchors Anchors { get; set; }

    private static bool hasInitialized = false;
    public static bool HasInitialized => hasInitialized;

    public static void InitializeGameData(GameMetaData _data)
    {
        hasInitialized = false;

        SessionState = new SessionState();
        EconomyData = _data.EconomyData ?? new Economy(100);
        PlacementData = _data.PlacementData;
        PlaceableData = _data.PlaceableData;

        // 코어 서비스 초기화
        EventBus = new GameEventBus();
        StaffRegistry = new StaffRegistry();
        TaskQueue = new TaskQueue();

        // 매니저/서비스 초기화
        EconomyService = new EconomyService(EconomyData);
        SessionService = new SessionService(SessionState);
        PlaceableService = new PlaceableService(PlaceableData);
        OrderService = new OrderService();

        // TaskAssigner는 EventBus, TaskQueue, StaffRegistry가 초기화된 후 생성
        TaskAssigner = new TaskAssigner(TaskQueue, StaffRegistry);

        AsyncOperationHandle handle = Addressables.LoadAssetAsync<PoolRegistry>("Assets/_Project/91_Data/PoolRegistry.asset");
        handle.Completed += (op) =>
        {
            PoolRegistry registry = op.Result as PoolRegistry;
            PoolService = new PoolService(registry);
            hasInitialized = true;
        };
    }

    public static GameMetaData GetSessionDataToMeta()
    {
        return new GameMetaData
        {
            PlacementData = PlacementData,
            PlaceableData = PlaceableData,
            EconomyData = EconomyData
        };
    }

    public static void SetPlacementData(PlacementData _placementData)
    {
        PlacementData = _placementData;
    }

    public static PlacementData GetPlacementData()
    {
        if (PlacementData == null)
            return null;
        return PlacementData;
    }

    // 안전 사용 헬퍼
    public static bool HasGameData => PlacementData != null;
}