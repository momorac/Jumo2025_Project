using UnityEngine;

public class GameAnchors : MonoBehaviour
{
    public Transform AgentRoot;

    public Transform[] CustomerSpawnPoints;

    void Start()
    {
        App.Anchors = this;
    }
}
