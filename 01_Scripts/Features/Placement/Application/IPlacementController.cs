using UnityEngine;

public interface IPlacementController
{
    bool CanPlace(Placement type);
    GameObject Place(Placement type, Vector3 pos, Quaternion rot);
}
