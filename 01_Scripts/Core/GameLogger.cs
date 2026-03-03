using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 로그 카테고리 (색상 구분)
/// </summary>
public enum LogCategory
{
    Staff,      // Staff 행동, 선택, 등록
    Customer,   // Customer 상태, 스폰
    Task,       // Task 생성, 배정, 완료
    Facility,   // 조리/자원 시설
    Economy,    // 주문, 재료, 레시피, 수익
    System,     // 세션, 저장, 스폰 시스템
    Input,      // 클릭, 포인팅
}

/// <summary>
/// 로그 레벨
/// </summary>
public enum LogLevel
{
    Error = 0,  // 항상 출력 (치명적 문제)
    Info = 1,  // 주요 이벤트 (배정, 완료, 생성 등)
    Verbose = 2,  // 상세 디버그 (상태 전환, 자원 변동 등)
}

/// <summary>
/// 중앙 로그 유틸리티.
/// 포맷: [Category] message
/// 카테고리별 색상, 레벨별 필터링 지원
/// </summary>
public static class GameLogger
{
    // ═══════════ 설정 ═══════════

    /// <summary>이 레벨 이하만 출력 (Error=0, Info=1, Verbose=2)</summary>
    public static LogLevel CurrentLevel = LogLevel.Verbose;

    /// <summary>카테고리별 활성 여부 (기본 전부 활성)</summary>
    private static readonly bool[] categoryEnabled = new bool[Enum.GetValues(typeof(LogCategory)).Length];

    static GameLogger()
    {
        for (int i = 0; i < categoryEnabled.Length; i++)
            categoryEnabled[i] = true;
    }

    // ═══════════ 카테고리별 색상 (hex) ═══════════

    private static string GetColor(LogCategory category)
    {
        return category switch
        {
            LogCategory.Staff => "#00ffffff",  // cyan
            LogCategory.Customer => "#0000ffff",  // blue
            LogCategory.Task => "#00ff00ff",  // lime
            LogCategory.Facility => "#ffa500ff",  // orange
            LogCategory.Economy => "#ffff00ff",  // yellow
            LogCategory.System => "#ff00ffff",  // magenta
            LogCategory.Input => "#800080ff",  // purple
            _ => "#FFFFFF",
        };
    }

    // ═══════════ 로그 메서드 ═══════════

    /// <summary>주요 이벤트 로그 (Level: Info)</summary>
    public static void Log(LogCategory category, string message)
    {
        if (CurrentLevel < LogLevel.Info) return;
        if (!categoryEnabled[(int)category]) return;

        string color = GetColor(category);
        Debug.Log($"<color={color}>[{category}]</color> {message}");
    }

    /// <summary>상세 디버그 로그 (Level: Verbose)</summary>
    [Conditional("UNITY_EDITOR")]
    public static void LogVerbose(LogCategory category, string message)
    {
        if (CurrentLevel < LogLevel.Verbose) return;
        if (!categoryEnabled[(int)category]) return;

        string color = GetColor(category);
        Debug.Log($"<color={color}>[{category}]</color> <color=#BDBDBD>{message}</color>");
    }

    /// <summary>경고 로그 (Level: Info)</summary>
    public static void LogWarning(LogCategory category, string message)
    {
        if (CurrentLevel < LogLevel.Info) return;
        if (!categoryEnabled[(int)category]) return;

        string color = GetColor(category);
        Debug.LogWarning($"<color={color}>[{category}]</color> {message}");
    }

    /// <summary>에러 로그 (항상 출력)</summary>
    public static void LogError(LogCategory category, string message)
    {
        string color = GetColor(category);
        Debug.LogError($"<color={color}>[{category}]</color> {message}");
    }

    // ═══════════ 설정 변경 ═══════════

    public static void SetLevel(LogLevel level) => CurrentLevel = level;
    public static void SetCategoryEnabled(LogCategory category, bool enabled) => categoryEnabled[(int)category] = enabled;
    public static void EnableAll() { for (int i = 0; i < categoryEnabled.Length; i++) categoryEnabled[i] = true; }
    public static void DisableAll() { for (int i = 0; i < categoryEnabled.Length; i++) categoryEnabled[i] = false; }
}
