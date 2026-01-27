using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using NUnit.Framework;

public class Pedestrian : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Transform start;
    private Transform target;

    private bool hasInitialized = false;

    private NavMeshAgent agent;
    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;

    public void OnSpawn()
    {
        if (!hasInitialized)
        {
            agent = GetComponent<NavMeshAgent>();
            hasInitialized = true;
        }

        if (agent == null || animator == null)
        {
            Debug.LogWarning($"{nameof(Pedestrian)}: NavMeshAgent 혹은 Animator가 없습니다.");
            return;
        }

        Debug.Log("<color=green>Pedestrian spawned.</color>");

        // 위치 랜덤 설정
        int randomIdx = Random.Range(0, spawnPoints.Length);
        start = spawnPoints[randomIdx];
        target = spawnPoints[(randomIdx + 1) % spawnPoints.Length];

        transform.position = start.position;
        agent.SetDestination(target.position);
        StartCoroutine(SetWalkingWhenMoving());
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
