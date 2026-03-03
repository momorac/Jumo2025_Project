using UnityEngine;

/// <summary>
/// 조리 시설 기초 구현. 
/// 개별 시설은 이를 상속하여 고유 기능 추가
/// </summary>
public abstract class CookingFacilityBase : MonoBehaviour, ICookingFacility, IClickable
{
    [Header("Facility Type")]
    [SerializeField] protected FacilityType facilityType;

    [Header("Resource Settings")]
    [SerializeField] protected int maxWater = 10;
    [SerializeField] protected int maxWood = 10;
    [SerializeField] protected int waterPerCook = 1;
    [SerializeField] protected int woodPerCook = 1;

    [Header("Current State")]
    [SerializeField] protected int currentWater;
    [SerializeField] protected int currentWood;

    // ICookingFacility 구현
    public FacilityType FacilityType => facilityType;
    public CookingFacilityType CookingType => facilityType.ToCookingType();
    public Transform Transform => transform;

    public virtual bool RequiresResources => facilityType.RequiresResources();
    public int CurrentWater => currentWater;
    public int CurrentWood => currentWood;
    public int MaxWater => maxWater;
    public int MaxWood => maxWood;

    public virtual bool CanCook => !RequiresResources ||
        (currentWater >= waterPerCook && currentWood >= woodPerCook);

    public bool NeedsWater => RequiresResources && currentWater < maxWater * 0.3f;
    public bool NeedsWood => RequiresResources && currentWood < maxWood * 0.3f;

    public float WaterRatio => maxWater > 0 ? (float)currentWater / maxWater : 1f;
    public float WoodRatio => maxWood > 0 ? (float)currentWood / maxWood : 1f;

    // IClickable 구현
    public virtual bool IsClickable => true;
    public virtual int ClickPriority => 5;

    protected virtual void Start()
    {
        OnPlaced();
    }

    protected virtual void OnDestroy()
    {
        OnRemoved();
    }

    public virtual void AddWater(int amount)
    {
        if (!RequiresResources) return;
        currentWater = Mathf.Min(currentWater + amount, maxWater);
        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: Water +{amount} ({currentWater}/{maxWater})");
    }

    public virtual void AddWood(int amount)
    {
        if (!RequiresResources) return;
        currentWood = Mathf.Min(currentWood + amount, maxWood);
        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: Wood +{amount} ({currentWood}/{maxWood})");
    }

    public virtual void ConsumeResources()
    {
        if (!RequiresResources) return;
        currentWater = Mathf.Max(0, currentWater - waterPerCook);
        currentWood = Mathf.Max(0, currentWood - woodPerCook);
        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: resources consumed (Water: {currentWater}, Wood: {currentWood})");

        // 자원 부족 이벤트 발행
        CheckResourceLevels();
    }

    protected virtual void CheckResourceLevels()
    {
        if (NeedsWater)
        {
            App.EventBus.Publish(new FacilityResourceLowEvent(this, FacilityResourceType.Water, WaterRatio));
        }
        if (NeedsWood)
        {
            App.EventBus.Publish(new FacilityResourceLowEvent(this, FacilityResourceType.Firewood, WoodRatio));
        }
    }

    public virtual void OnPlaced()
    {
        App.PlaceableService?.RegisterCookingFacility(this);
        GameLogger.Log(LogCategory.Facility, $"{name} ({CookingType}) placed");
    }

    public virtual void OnRemoved()
    {
        App.PlaceableService?.UnregisterCookingFacility(this);
        GameLogger.Log(LogCategory.Facility, $"{name} ({CookingType}) removed");
    }

    public abstract void OnClicked(Vector3 hitPoint);
}
