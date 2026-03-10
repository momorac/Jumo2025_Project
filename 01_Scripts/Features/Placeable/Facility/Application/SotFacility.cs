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

        App.EventBus.Publish(new CookingFacilityClickedEvent(this));
    }
}
