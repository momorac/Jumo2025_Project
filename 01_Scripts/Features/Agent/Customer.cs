using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour, IPooled
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;
    private Transform start;
    private Transform target;
    private bool hasInitialized = false;

    public void OnGet()
    {
        if (!hasInitialized)
        {
            agent = GetComponent<NavMeshAgent>();
            hasInitialized = true;
        }

        if (agent == null || animator == null)
        {
            Debug.LogWarning($"{nameof(Customer)}: NavMeshAgent 혹은 Animator가 없습니다.");
            return;
        }

        Debug.Log("<color=green>CUSTOMER spawned.</color>");
    }

    public void OnRelease()
    {
        Debug.Log("<color=yellow>CUSTOMER released back to pool.</color>");
    }


    // public void SeatAt(Table seat)

}
