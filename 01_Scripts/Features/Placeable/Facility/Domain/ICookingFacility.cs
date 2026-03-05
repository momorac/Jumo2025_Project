using UnityEngine;

/// <summary>
/// 조리 시설 공통 인터페이스
/// </summary>
public interface ICookingFacility
{
    // 기본 정보
    FacilityType FacilityType { get; }
    CookingFacilityType CookingType { get; }
    Transform Transform { get; }

    // 자원 상태
    bool RequiresResources { get; }
    int CurrentWater { get; }
    int CurrentWood { get; }
    int MaxWater { get; }
    int MaxWood { get; }

    // 상태 체크
    bool CanCook { get; }
    bool NeedsWater { get; }
    bool NeedsWood { get; }

    // 자원 관리
    void AddWater(int amount);
    void AddWood(int amount);
    void ConsumeResources();

    // 생명주기
    void OnPlaced();
    void OnRemoved();
}
