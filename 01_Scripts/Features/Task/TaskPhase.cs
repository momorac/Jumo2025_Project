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

    /// <summary>도착 후 재생할 애니메이션 트리거</summary>
    public string AnimationTrigger;

    /// <summary>실행 시간 (초)</summary>
    public float Duration;

    /// <summary>실행 시간의 중간 지점에서 호출되는 로직</summary>
    public System.Action<Staff> OnExecute;

    public TaskPhase(Transform moveTarget, float duration, System.Action<Staff> onExecute = null, string animationTrigger = null)
    {
        MoveTarget = moveTarget;
        Duration = duration;
        OnExecute = onExecute;
        AnimationTrigger = animationTrigger;
    }
}
