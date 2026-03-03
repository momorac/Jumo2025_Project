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

    // 컨트롤러 접근자
    public StaffController Controller => controller;

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
        GameLogger.Log(LogCategory.Staff, $"Staff {name} selected");
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
        GameLogger.Log(LogCategory.Staff, $"{name}: carrying {resourceType} x{amount}");
        // TODO: 운반 비주얼 활성화
    }

    /// <summary>들고 있는 자원 내려놓기 (조리 시설에 전달 후 호출)</summary>
    public void DropResource()
    {
        GameLogger.Log(LogCategory.Staff, $"{name}: delivered {carryingResourceType} x{carryingAmount}");
        carryingResourceType = FacilityResourceType.None;
        carryingAmount = 0;
        // TODO: 운반 비주얼 비활성화
    }
}
