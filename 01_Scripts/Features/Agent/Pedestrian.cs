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
            GameLogger.LogWarning(LogCategory.System, $"{nameof(Pedestrian)}: NavMeshAgent or Animator missing");
            return;
        }

        GameLogger.LogVerbose(LogCategory.System, $"{name} spawned");

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
        GameLogger.LogVerbose(LogCategory.System, $"{name} released to pool");
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
