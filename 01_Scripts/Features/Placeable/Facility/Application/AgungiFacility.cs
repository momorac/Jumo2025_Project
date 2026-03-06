using UnityEngine;

/// <summary>가마솥 - 반찬, 국밥/찌개</summary>
public class AgungiFacility : CookingFacilityBase
{

    private void Awake()
    {
        facilityType = FacilityType.Agungi;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        GameLogger.Log(LogCategory.Input, $"Agungi clicked (CanCook: {CanCook})");
        // TODO: 조리 메뉴 UI 표시
    }
}
