using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Pedestrian : MonoBehaviour, IPooled
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private Transform[] spawnPoints => App.Anchors.CustomerSpawnPoints;
    private Transform start;
    private Transform target;
    private bool hasInitialized = false;

    public event Action OnWalkComplete;

    public void OnGet()
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
        StartCoroutine(TrackWalkingCoroutine());
    }

    public void OnRelease()
    {
        OnWalkComplete = null;
        Debug.Log("<color=yellow>Pedestrian released back to pool.</color>");
    }

    private IEnumerator TrackWalkingCoroutine()
    {
        animator.SetBool("IsWalking", true);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        animator.SetBool("IsWalking", false);
        OnWalkComplete?.Invoke();
    }
}
