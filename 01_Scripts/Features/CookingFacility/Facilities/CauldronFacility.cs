using UnityEngine;

/// <summary>가마솥 - 반찬, 국밥/찌개</summary>
public class CauldronFacility : CookingFacilityBase
{

    private void Awake()
    {
        facilityType = FacilityType.Cauldron;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=magenta>가마솥 클릭됨 - 반찬/찌개 조리 가능 (CanCook: {CanCook})</color>");
        // TODO: 조리 메뉴 UI 표시
    }
}
