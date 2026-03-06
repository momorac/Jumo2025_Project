using UnityEngine;

/// <summary> 솥 - 밥, 국 조리</summary>
public class PotFacility : CookingFacilityBase
{
    private void Awake()
    {
        facilityType = FacilityType.Sot;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        GameLogger.Log(LogCategory.Input, $"Pot clicked (CanCook: {CanCook})");
        // TODO: 조리 메뉴 UI 표시
    }
}
