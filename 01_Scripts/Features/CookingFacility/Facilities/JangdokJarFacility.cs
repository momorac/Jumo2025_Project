using UnityEngine;

/// <summary> 장독대 - 김치 발효 (물/장작 불필요)</summary>
public class JangdokJarFacility : CookingFacilityBase
{
    private bool isFermenting = false;

    private void Awake()
    {
        facilityType = FacilityType.JangdokJar;
    }

    /// <summary> 장독대는 자원 불필요</summary>
    public override bool RequiresResources => false;

    /// <summary>발효 중이 아니면 항상 조리(발효) 가능 </summary>
    public override bool CanCook => !isFermenting;

    public override void OnClicked(Vector3 hitPoint)
    {
        GameLogger.Log(LogCategory.Input, $"JangdokJar clicked");
        // TODO: 김치 발효 메뉴 UI 표시
    }

}
