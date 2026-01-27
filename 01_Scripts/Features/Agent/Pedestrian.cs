using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Pedestrian : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogWarning($"{nameof(Pedestrian)}: NavMeshAgent가 없습니다.");
            return;
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
            if (animator != null)
            {
                StartCoroutine(SetWalkingWhenMoving());
            }
        }
        else
        {
            Debug.LogWarning($"{nameof(Pedestrian)}: target이 지정되지 않았습니다.");
        }
    }

    private IEnumerator SetWalkingWhenMoving()
    {
        if (agent == null || animator == null)
            yield break;

        animator.SetBool("IsWalking", true);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        animator.SetBool("IsWalking", false);
    }
}
