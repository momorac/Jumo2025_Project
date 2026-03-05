using UnityEngine;

/// <summary>
/// 자원 시설 베이스 클래스 (우물, 장작더미)
/// 이벤트 발생 시 고정된 양의 자원을 반환
/// </summary>
public abstract class ResourceFacilityBase : MonoBehaviour, IClickable
{
    [Header("Resource Settings")]
    [SerializeField] protected FacilityType facilityType;
    [SerializeField] protected FacilityResourceType providedResourceType;
    [SerializeField] protected int amountPerCollect = 5; // 수집 시 반환되는 고정 양
    [SerializeField] protected float collectDuration = 3f;  // 수집 모션 재생 시간

    [Header("Links")]
    [SerializeField] private Transform targetTransform; // 클릭 시 이동 목표 지점 (예: 자원 위치)
    [SerializeField] protected Placeable placeable;

    public Transform TargetTransform => targetTransform;

    public FacilityType FacilityType => facilityType;
    public FacilityResourceType ProvidedResourceType => providedResourceType;
    public int AmountPerCollect => amountPerCollect;
    public float CollectDuration => collectDuration;

    /// <summary> 자원 수집 - 고정된 양 반환 </summary>
    /// <returns>수집된 양</returns>
    public virtual int CollectResource()
    {
        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: {providedResourceType} x{amountPerCollect} collected");
        return amountPerCollect;
    }

    /// <summary> 자원이 있는지 여부 (무한 자원이므로 항상 true) </summary>
    public bool HasResource => true;

    // IClickable 구현
    public virtual bool IsClickable => true;
    public virtual int ClickPriority => 3;

    public abstract void OnClicked(Vector3 hitPoint);
}
