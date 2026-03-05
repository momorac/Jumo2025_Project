using UnityEngine;

/// <summary>
/// Task의 개별 실행 단계.
/// 이동 목표 + 도착 후 행동을 하나의 단위로 묶음.
/// Task는 1개 이상의 Phase로 구성됨.
/// </summary>
public class TaskPhase
{
    /// <summary>이동할 위치 (null이면 이동 없이 즉시 실행)</summary>
    public Transform MoveTarget;

    /// <summary>이 Phase가 이동을 먼저 수행하는지 여부</summary>
    public bool WillMoveFirst => MoveTarget != null;

    /// <summary>실행 시간 (초)</summary>
    public float Duration;

    /// <summary>Phase 실행 시작 시 호출 (도착 직후, 애니메이션 재생 후)</summary>
    public System.Action<Staff> OnStart;

    /// <summary>Phase 실행 로직. Duration 경과 후 호출</summary>
    public System.Action<Staff> OnExecute;

    /// <summary>Phase 종료 직전 호출 (다음 Phase 또는 Task 완료로 넘어가기 전)</summary>
    public System.Action<Staff> OnEnd;

    public TaskPhase(Transform moveTarget, float duration,
        System.Action<Staff> onStart = null,
         System.Action<Staff> onExecute = null,
        System.Action<Staff> onEnd = null)
    {
        MoveTarget = moveTarget;
        Duration = duration;
        OnExecute = onExecute;
        OnStart = onStart;
        OnEnd = onEnd;
    }
}
