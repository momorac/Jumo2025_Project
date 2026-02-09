using UnityEngine;

/// <summary>
/// Staff 에이전트
/// StaffController를 통해 FSM 관리
/// IClickable 구현으로 수동 선택 지원
/// </summary>
[RequireComponent(typeof(StaffController))]
public class Staff : MonoBehaviour, IClickable
{
    [Header("Settings")]
    [SerializeField] private int clickPriority = 5;

    private StaffController controller;

    // IClickable 구현
    public bool IsClickable => true;
    public int ClickPriority => clickPriority;

    // 상태 접근자
    public bool IsIdle => controller?.IsIdle ?? false;
    public IStaffTask CurrentTask => controller?.CurrentTask;

    private void Awake()
    {
        controller = GetComponent<StaffController>();
    }

    /// <summary> 클릭 시 Staff 선택</summary>
    public void OnClicked(Vector3 hitPoint)
    {
        App.StaffRegistry.SelectStaff(this);
        Debug.Log($"<color=cyan>Staff {name} clicked and selected</color>");
    }

    /// <summary>작업 배정 (StaffController에 위임)</summary>
    public void AssignTask(IStaffTask task)
    {
        controller?.AssignTask(task);
    }

    /// <summary> 특정 위치로 이동 (StaffController에 위임)</summary>
    public void MoveTo(Vector3 position)
    {
        controller?.MoveTo(position);
    }
}
