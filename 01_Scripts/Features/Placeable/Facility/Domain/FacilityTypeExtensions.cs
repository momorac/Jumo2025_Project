using System.Collections.Generic;

/// <summary>FacilityType ↔ CookingFacilityType 매핑 확장 메서드</summary>
public static class FacilityTypeExtensions
{
    // FacilityType → CookingFacilityType
    private static readonly Dictionary<FacilityType, CookingFacilityType> ToCookingMap = new()
    {
        { FacilityType.Sot, CookingFacilityType.Pot },
        { FacilityType.Agungi, CookingFacilityType.Agungi },
        { FacilityType.Brazier, CookingFacilityType.Brazier },
        { FacilityType.JangdokJar, CookingFacilityType.JangdokJar },
    };

    // CookingFacilityType → FacilityType
    private static readonly Dictionary<CookingFacilityType, FacilityType> ToFacilityMap = new()
    {
        { CookingFacilityType.Pot, FacilityType.Sot },
        { CookingFacilityType.Agungi, FacilityType.Agungi },
        { CookingFacilityType.Brazier, FacilityType.Brazier },
        { CookingFacilityType.JangdokJar, FacilityType.JangdokJar },
    };

    // FacilityType → 자원 출력 매핑
    private static readonly Dictionary<FacilityType, FacilityResourceType> ResourceOutputMap = new()
    {
        { FacilityType.Well, FacilityResourceType.Water },
        { FacilityType.Stump, FacilityResourceType.Firewood },
    };

    /// <summary>FacilityType → CookingFacilityType 변환 </summary>
    public static CookingFacilityType ToCookingType(this FacilityType type)
    {
        return ToCookingMap.TryGetValue(type, out var cookingType)
            ? cookingType
            : CookingFacilityType.None;
    }

    /// <summary>CookingFacilityType → FacilityType 변환 </summary>
    public static FacilityType ToFacilityType(this CookingFacilityType type)
    {
        return ToFacilityMap.TryGetValue(type, out var facilityType)
            ? facilityType
            : FacilityType.None;
    }

    /// <summary>조리 시설 여부 확인</summary>
    public static bool IsCookingFacility(this FacilityType type)
    {
        return ToCookingMap.ContainsKey(type);
    }

    /// <summary>자원 시설 여부 확인</summary>
    public static bool IsResourceFacility(this FacilityType type)
    {
        return ResourceOutputMap.ContainsKey(type);
    }

    /// <summary>자원(물/장작) 필요 여부 (장독대는 false)</summary>
    public static bool IsResourceRequired(this FacilityType type)
    {
        return type.IsCookingFacility() && type != FacilityType.JangdokJar;
    }

    /// <summary>자원 시설이 제공하는 자원 타입</summary>
    public static FacilityResourceType GetOutputResource(this FacilityType type)
    {
        return ResourceOutputMap.TryGetValue(type, out var resource)
            ? resource
            : FacilityResourceType.None;
    }
}
