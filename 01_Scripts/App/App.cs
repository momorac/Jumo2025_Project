using System;

public static class App
{
    // 전역 모델
    public static GameMetaData GameData { get; private set; }

    // 세션 서비스: Economy
    public static EconomyService EconomyService { get; private set; }

    // 변경 알림(필요 시)
    public static event Action<GameMetaData> GameDataChanged;

    // 등록/갱신
    public static void SetGameData(GameMetaData data)
    {
        GameData = data;
        GameDataChanged?.Invoke(GameData);
    }

    // 세션 서비스 등록/해제
    public static void SetEconomyService(EconomyService service)
    {
        EconomyService = service;
    }

    public static void SetPlacementData(PlacementData placementData)
    {
        GameData.PlacementData = placementData;
        GameDataChanged?.Invoke(GameData);
    }

    public static PlacementData GetPlacementData()
    {
        if (GameData == null)
            return null;
        return GameData?.PlacementData;
    }

    // 안전 사용 헬퍼
    public static bool HasGameData => GameData != null;
}