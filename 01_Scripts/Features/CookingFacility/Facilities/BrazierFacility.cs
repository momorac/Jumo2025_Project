using UnityEngine;

/// <summary> 화로 - 전, 구이 요리</summary>
public class BrazierFacility : CookingFacilityBase
{

    private void Awake()
    {
        facilityType = FacilityType.Brazier;
    }

    /// <summary>화로는 불이 켜져 있어야 조리 가능</summary>
    public override bool CanCook => base.CanCook;

    public override void OnClicked(Vector3 hitPoint)
    {
        Debug.Log($"<color=magenta>화로 클릭됨 - 전/구이 조리 가능 (CanCook: {CanCook}");

    }
}
