
using UnityEngine;

public static class App
{
    // 전역 모델
    // public static GameMetaData GameData { get; private set; }
    public static SessionData SessionData;
    public static PlacementData PlacementData;
    public static PlaceableData PlaceableData;

    public static EconomyService EconomyService { get; private set; }

    public static PoolService Pool;


    // 초기화
    public static void InitializeGameData(GameMetaData _data)
    {
        Debug.Log("App InitializeGameData");
        SessionData = new SessionData();
        PlacementData = _data.PlacementData;
        PlaceableData = _data.PlaceableData;

        // 매니저/서비스 초기화
        EconomyService = new EconomyService(_data.EconomyData.Money);
        Pool = new PoolService();
    }

    public static GameMetaData GetSessionGameData()
    {
        return new GameMetaData
        {
            PlacementData = PlacementData,
            PlaceableData = PlaceableData,
            EconomyData = EconomyService.GetEconomyData()
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