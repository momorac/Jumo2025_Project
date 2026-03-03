
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
    private static IngredientData IngredientData;
    private static RecipeData RecipeData;

    // 이벤트 관련
    public static TaskQueue TaskQueue { get; private set; }
    public static TaskAssigner TaskAssigner { get; private set; }
    public static StaffRegistry StaffRegistry { get; private set; }
    public static GameAnchors Anchors { get; set; }

    // 서비스/매니저
    public static SessionService SessionService { get; private set; }
    public static EconomyService EconomyService { get; private set; }
    public static PlaceableService PlaceableService { get; private set; }
    public static PoolService PoolService { get; private set; }
    public static GameEventBus EventBus { get; private set; }
    public static OrderService OrderService { get; private set; }
    public static IngredientService IngredientService { get; private set; }
    public static RecipeService RecipeService { get; private set; }

    private static bool hasInitialized = false;
    public static bool HasInitialized => hasInitialized;

    public static void InitializeGameData(GameMetaData _data)
    {
        hasInitialized = false;

        SessionState = new SessionState();
        EconomyData = _data.EconomyData ?? new Economy(100);
        PlacementData = _data.PlacementData;
        PlaceableData = _data.PlaceableData;
        IngredientData = _data.IngredientData ?? new IngredientData();
        RecipeData = _data.RecipeData ?? new RecipeData();

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

        // PoolRegistry 로드 및 PoolService 초기화
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<PoolRegistry>("Assets/_Project/91_Data/PoolRegistry.asset");
        handle.Completed += (op) =>
        {
            PoolRegistry registry = op.Result as PoolRegistry;
            PoolService = new PoolService(registry);
        };

        // IngredientRegistry 로드
        AsyncOperationHandle ingredientHandle = Addressables.LoadAssetAsync<IngredientRegistry>("Assets/_Project/91_Data/IngredientRegistry.asset");
        ingredientHandle.Completed += (ingredientOp) =>
        {
            IngredientRegistry ingredientRegistry = ingredientOp.Result as IngredientRegistry;
            IngredientService = new IngredientService(IngredientData, ingredientRegistry);

        };

        // RecipeRegistry 로드
        AsyncOperationHandle recipeHandle = Addressables.LoadAssetAsync<RecipeRegistry>("Assets/_Project/91_Data/RecipeRegistry.asset");
        recipeHandle.Completed += (recipeOp) =>
        {
            RecipeRegistry recipeRegistry = recipeOp.Result as RecipeRegistry;
            RecipeService = new RecipeService(RecipeData, recipeRegistry);
            hasInitialized = true;
        };
    }

    public static GameMetaData GetSessionDataToMeta()
    {
        return new GameMetaData
        {
            PlacementData = PlacementData,
            PlaceableData = PlaceableData,
            EconomyData = EconomyData,
            IngredientData = IngredientData,
            RecipeData = RecipeData
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