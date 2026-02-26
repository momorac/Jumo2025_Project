using UnityEngine;

/// <summary>
/// 자원 시설 베이스 클래스 (우물, 장작더미)
/// </summary>
public abstract class ResourceFacilityBase : MonoBehaviour, IClickable
{
    [Header("Resource Settings")]
    [SerializeField] protected FacilityType facilityType;
    [SerializeField] protected FacilityResourceType providedResourceType;
    [SerializeField] protected int maxResource = 100;
    [SerializeField] protected int currentResource = 100;
    [SerializeField] protected float regenerationRate = 0f; // 초당 재생량 (0이면 재생 안함)

    [Header("Links")]
    [SerializeField] protected Placeable placeable;

    public FacilityType FacilityType => facilityType;
    public FacilityResourceType ProvidedResourceType => providedResourceType;
    public int CurrentResource => currentResource;
    public int MaxResource => maxResource;
    public float ResourceRatio => maxResource > 0 ? (float)currentResource / maxResource : 0f;

    protected virtual void Update()
    {
        // 자원 재생
        if (regenerationRate > 0 && currentResource < maxResource)
        {
            currentResource = Mathf.Min(maxResource, currentResource + Mathf.RoundToInt(regenerationRate * Time.deltaTime));
        }
    }

    /// <summary> 자원 수집 </summary>
    /// <param name="amount">수집할 양</param>
    /// <returns>실제 수집된 양</returns>
    public virtual int CollectResource(int amount)
    {
        int collected = Mathf.Min(amount, currentResource);
        currentResource -= collected;
        Debug.Log($"<color=blue>{name}: {providedResourceType} {collected}만큼 수집됨 (남은 양: {currentResource})</color>");
        return collected;
    }

    /// <summary> 자원 채우기 (관리용) </summary>
    /// <param name="amount">채울 양</param>
    public virtual void RefillResource(int amount)
    {
        currentResource = Mathf.Min(maxResource, currentResource + amount);
        Debug.Log($"<color=green>{name}: {providedResourceType} 보충됨 (현재: {currentResource})</color>");
    }

    /// <summary> 자원이 있는지 여부 </summary>
    public bool HasResource => currentResource > 0;

    // IClickable 구현
    public virtual bool IsClickable => HasResource;
    public virtual int ClickPriority => 3;

    public abstract void OnClicked(Vector3 hitPoint);
}
