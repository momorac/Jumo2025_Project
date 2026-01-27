using UnityEngine;

public class GameAnchors : MonoBehaviour
{
    public Transform AgentRoot;


    void Start()
    {
        App.Anchors = this;
    }
}
