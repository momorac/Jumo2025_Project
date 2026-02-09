using UnityEngine;

/// <summary>
/// 클릭 가능한 오브젝트 인터페이스
/// PointingSystem에서 Raycast로 감지하여 OnClicked 호출
/// </summary>
public interface IClickable
{
    /// <summary> 클릭되었을 때 호출됩니다 </summary>
    /// <param name="hitPoint">클릭된 월드 좌표</param>
    void OnClicked(Vector3 hitPoint);

    /// <summary> 현재 클릭 가능한 상태인지 반환합니다 </summary>
    bool IsClickable { get; }

    /// <summary> 클릭 우선순위 (높을수록 우선) / 동일 위치에 여러 IClickable이 있을 때 사용 </summary>
    int ClickPriority { get; }
}
