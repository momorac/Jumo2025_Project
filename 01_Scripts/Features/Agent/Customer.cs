using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

        // 위치 랜덤 설정
        int randomIdx = Random.Range(0, spawnPoints.Length);
        start = spawnPoints[randomIdx];
        target = spawnPoints[(randomIdx + 1) % spawnPoints.Length];

        transform.position = start.position;
    }

    public void SetSeat(Transform seat)
    {
        agent.SetDestination(seat.position);
        StartCoroutine(TrackSeatingCoroutine(seat));
    }

    public void OnRelease()
    {
        Debug.Log("<color=yellow>CUSTOMER released back to pool.</color>");
    }


    private IEnumerator TrackSeatingCoroutine(Transform seat)
    {
        animator.SetBool("IsWalking", true);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        agent.enabled = false;
        animator.SetBool("IsWalking", false);
        transform.SetPositionAndRotation(seat.position, seat.rotation);


        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("SitTrigger");
    }
}
