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

    // 자원 운반 상태
    private FacilityResourceType carryingResourceType = FacilityResourceType.None;
    private int carryingAmount = 0;

    // IClickable 구현
    public bool IsClickable => true;
    public int ClickPriority => clickPriority;

    // 상태 접근자
    public bool IsIdle => controller?.IsIdle ?? false;
    public IStaffTask CurrentTask => controller?.CurrentTask;
    public bool IsCarrying => carryingResourceType != FacilityResourceType.None;
    public FacilityResourceType CarryingResourceType => carryingResourceType;
    public int CarryingAmount => carryingAmount;

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

    /// <summary>자원을 들기</summary>
    public void PickUpResource(FacilityResourceType resourceType, int amount)
    {
        carryingResourceType = resourceType;
        carryingAmount = amount;
        Debug.Log($"<color=cyan>{name}: {resourceType} {amount}개 운반 시작</color>");
        // TODO: 운반 비주얼 활성화
    }

    /// <summary>들고 있는 자원 내려놓기 (조리 시설에 전달 후 호출)</summary>
    public void DropResource()
    {
        Debug.Log($"<color=cyan>{name}: {carryingResourceType} {carryingAmount}개 전달 완료</color>");
        carryingResourceType = FacilityResourceType.None;
        carryingAmount = 0;
        // TODO: 운반 비주얼 비활성화
    }
}
