
using UnityEngine;

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

    public static GameAnchors Anchors { get; set; }


    public static void InitializeGameData(GameMetaData _data)
    {
        Debug.Log("App InitializeGameData");
        SessionState = new SessionState();
        EconomyData = _data.EconomyData ?? new Economy(100);
        PlacementData = _data.PlacementData;
        PlaceableData = _data.PlaceableData;

        // 매니저/서비스 초기화
        EconomyService = new EconomyService(EconomyData);
        SessionService = new SessionService(SessionState);
        PlaceableService = new PlaceableService(PlaceableData);
        PoolService = new PoolService();
    }

    public static GameMetaData GetSessionGameData()
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