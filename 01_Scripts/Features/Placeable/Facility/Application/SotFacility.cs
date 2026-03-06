using UnityEngine;

/// <summary> 솥 or </summary>
public class SotFacility : CookingFacilityBase
{
    private void Awake()
    {
        facilityType = FacilityType.Sot;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        GameLogger.Log(LogCategory.Input, $"Sot clicked (CanCook: {CanCook})");
        // TODO: 조리 메뉴 UI 표시
    }
}
