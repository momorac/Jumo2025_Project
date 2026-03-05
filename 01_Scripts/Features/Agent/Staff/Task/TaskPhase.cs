using UnityEngine;

/// <summary>
/// Task의 개별 실행 단계.
/// 이동 목표 + 비주얼 데이터 + 비즈니스 로직을 하나의 단위로 묶음.
/// Task는 1개 이상의 Phase로 구성됨.
/// 비주얼(애니메이션/Prop)은 데이터로 선언하고, ExecutingTaskState가 처리.
/// </summary>
public class TaskPhase
{
    /// <summary>이동할 위치 (null이면 이동 없이 즉시 실행)</summary>
    public Transform MoveTarget;

    /// <summary>이 Phase가 이동을 먼저 수행하는지 여부</summary>
    public bool WillMoveFirst => MoveTarget != null;

    /// <summary>실행 시간 (초)</summary>
    public float Duration;

    // ── 비주얼 데이터 (선언형) ──

    /// <summary>실행 시 재생할 애니메이션 트리거 (null이면 재생 안함)</summary>
    public string AnimationTrigger;

    /// <summary>실행 시 활성화할 Prop (None이면 Prop 없음)</summary>
    public StaffPropId PropId;

    // ── 비즈니스 로직 콜백 ──

    /// <summary>Phase 실행 시작 시 호출 (커스텀 로직이 필요한 경우만)</summary>
    public System.Action<Staff> OnStart;

    /// <summary>Phase 실행 로직. Duration 경과 후 호출</summary>
    public System.Action<Staff> OnExecute;

    /// <summary>Phase 종료 직전 호출 (다음 Phase 또는 Task 완료로 넘어가기 전)</summary>
    public System.Action<Staff> OnEnd;

    public TaskPhase(
        Transform moveTarget,
        float duration,
        string animationTrigger = null,
        StaffPropId propId = StaffPropId.None,
        System.Action<Staff> onStart = null,
        System.Action<Staff> onExecute = null,
        System.Action<Staff> onEnd = null)
    {
        MoveTarget = moveTarget;
        Duration = duration;
        AnimationTrigger = animationTrigger;
        PropId = propId;
        OnStart = onStart;
        OnExecute = onExecute;
        OnEnd = onEnd;
    }
}
