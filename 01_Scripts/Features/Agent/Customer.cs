using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Customer : MonoBehaviour, IPooled
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;
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
        transform.position = spawnPoints[randomIdx].position;
    }

    public void OnRelease()
    {
        Debug.Log("<color=yellow>CUSTOMER released back to pool.</color>");
    }

    public void SetSeatDealy(Transform seat, float delay)
    {
        StartCoroutine(MoveToSeatCoroutine(seat, delay));
    }

    private IEnumerator MoveToSeatCoroutine(Transform seat, float delay)
    {
        yield return new WaitForSeconds(delay);

        agent.SetDestination(seat.position);
        animator.SetBool("IsWalking", true);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        agent.enabled = false;

        // 현재 위치부터 seat.position 자연스럽게 lerp
        transform.LookAt(seat.position);

        Vector3 startPos = transform.position;
        Vector3 targetPos = seat.position;

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        animator.SetBool("IsWalking", false);


        transform.SetPositionAndRotation(targetPos, seat.rotation);
        animator.SetTrigger("SitTrigger");
    }
}
