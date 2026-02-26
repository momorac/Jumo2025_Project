using UnityEngine;

/// <summary> 솥 - 밥, 국 조리</summary>
public class PotFacility : CookingFacilityBase
{
    private void Awake()
    {
        facilityType = FacilityType.Pot;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=magenta>솥 클릭됨 - 밥/국 조리 가능 (CanCook: {CanCook})</color>");
        // TODO: 조리 메뉴 UI 표시
    }
}
