using UnityEngine;

public class GameAnchors : MonoBehaviour
{
    public Transform PedestrianRoot;
    public Transform CustomerRoot;
    public Transform UIRoot;
    public Transform UI_BubbleRoot;

    public Transform[] CustomerSpawnPoints;

    void Start()
    {
        App.Anchors = this;
    }
}
