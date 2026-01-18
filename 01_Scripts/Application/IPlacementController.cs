using UnityEngine;

public interface IPlacementController
{
    bool CanPlace(FacilityType type);
    GameObject Place(FacilityType type, Vector3 pos, Quaternion rot);
}
