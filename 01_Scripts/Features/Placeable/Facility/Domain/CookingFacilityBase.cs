using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

/// <summary>
/// 조리 시설 기초 구현. 
/// 개별 시설은 이를 상속하여 고유 기능 추가
/// </summary>
public abstract class CookingFacilityBase : MonoBehaviour, ICookingFacility, IClickable
{
    [Header("Facility Type")]
    [SerializeField] protected FacilityType facilityType;

    [Header("Resource Settings")]
    [SerializeField] protected int waterPerCook = 1;
    [SerializeField] protected int woodPerCook = 1;
    [SerializeField] private Transform targetTransform; // 클릭 시 이동 목표 지점 (예: 조리대 위치)
    [SerializeField] private GameObject waterVisual;
    [SerializeField] private GameObject woodVisual;


    [Header("Current State")]
    [SerializeField] protected int currentWater;
    [SerializeField] protected int currentWood;

    // ICookingFacility 구현
    public FacilityType FacilityType => facilityType;
    public CookingFacilityType CookingType => facilityType.ToCookingType();
    public Transform Transform => transform;
    public Transform TargetTransform => targetTransform;

    public virtual bool IsResourceRequired => facilityType.IsResourceRequired();
    public int CurrentWater => currentWater;
    public int CurrentWood => currentWood;

    public virtual bool CanCook => !IsResourceRequired || (currentWater >= waterPerCook && currentWood >= woodPerCook);

    public bool IsWaterNeeded => IsResourceRequired && currentWater < waterPerCook;
    public bool IsWoodNeeded => IsResourceRequired && currentWood < woodPerCook;

    public float WaterRatio => waterPerCook > 0 ? Mathf.Clamp01((float)currentWater / waterPerCook) : 1f;
    public float WoodRatio => woodPerCook > 0 ? Mathf.Clamp01((float)currentWood / woodPerCook) : 1f;

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
        if (!IsResourceRequired) return;
        currentWater += amount;
        SetWaterVisual(currentWater > 0);

        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: Water +{amount} ({currentWater})");
    }

    public virtual void AddWood(int amount)
    {
        if (!IsResourceRequired) return;
        currentWood += amount;
        SetWoodVisual(currentWood > 0);

        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: Wood +{amount} ({currentWood})");
    }

    public virtual void ConsumeResources()
    {
        if (!IsResourceRequired) return;
        currentWater = Mathf.Max(0, currentWater - waterPerCook);
        currentWood = Mathf.Max(0, currentWood - woodPerCook);
        GameLogger.LogVerbose(LogCategory.Facility, $"{name}: resources consumed (Water: {currentWater}, Wood: {currentWood})");

        // 자원 부족 이벤트 발행
        CheckResourceLevels();
    }

    protected virtual void CheckResourceLevels()
    {
        if (IsWaterNeeded)
        {
            SetWaterVisual(false);
            App.EventBus.Publish(new FacilityResourceLowEvent(this, FacilityResourceType.Water, WaterRatio));
        }
        if (IsWoodNeeded)
        {
            SetWoodVisual(false);
            App.EventBus.Publish(new FacilityResourceLowEvent(this, FacilityResourceType.Firewood, WoodRatio));
        }
    }

    protected virtual void SetWaterVisual(bool hasWater)
    {
        if (waterVisual != null)
        {
            waterVisual.SetActive(hasWater);
        }
    }

    protected virtual void SetWoodVisual(bool hasWood)
    {
        if (woodVisual != null)
        {
            woodVisual.SetActive(hasWood);
        }
    }

    public virtual void OnPlaced()
    {
        SetWaterVisual(currentWater > 0);
        SetWoodVisual(currentWood > 0);
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
