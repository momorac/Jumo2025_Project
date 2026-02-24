using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 클릭 이벤트 중앙 관리 시스템
/// Raycast로 IClickable 오브젝트 감지 및 이벤트 발행
/// </summary>
public class PointingSystem : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableMask;
    [SerializeField] private float rayMaxDistance = 100f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = false;

    private Staff selectedStaff;
    private bool isStaffSelected;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        // Staff 선택 이벤트 구독
        App.EventBus.Subscribe<StaffSelectedEvent>(OnStaffSelected);
    }

    private void OnDestroy()
    {
        if (App.EventBus != null)
        {
            App.EventBus.Unsubscribe<StaffSelectedEvent>(OnStaffSelected);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        // 우클릭으로 Staff 선택 해제
        if (Input.GetMouseButtonDown(1))
        {
            ClearStaffSelection();
        }
    }

    private void HandleClick()
    {
        // UI 위를 클릭한 경우 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayMaxDistance, Color.yellow, 1f);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, clickableMask))
        {
            // IClickable 컴포넌트 검색 (본인 또는 부모에서)
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable == null)
            {
                clickable = hit.collider.GetComponentInParent<IClickable>();
            }

            if (clickable != null && clickable.IsClickable)
            {
                clickable.OnClicked(hit.point);

                // Staff가 아닌 곳 클릭 시 목적지 이벤트 발행 (선택된 Staff 또는 기본 Staff 이동)
                if (!(clickable is Staff))
                {
                    App.EventBus.Publish(new DestinationClickedEvent(hit.point, clickable));
                    ClearStaffSelection();
                }
            }
            else
            {
                // IClickable이 아닌 곳을 클릭해도 목적지로 사용 (선택된 Staff 또는 기본 Staff 이동)
                App.EventBus.Publish(new DestinationClickedEvent(hit.point, null));
                ClearStaffSelection();
            }
        }
    }

    private void OnStaffSelected(StaffSelectedEvent evt)
    {
        selectedStaff = evt.Staff;
        isStaffSelected = true;
        Debug.Log($"<color=cyan>Staff selected: {evt.Staff.name}</color>");
    }

    private void ClearStaffSelection()
    {
        if (isStaffSelected)
        {
            selectedStaff = null;
            isStaffSelected = false;
            Debug.Log("<color=cyan>Staff selection cleared</color>");
        }
    }

    /// <summary>
    /// 현재 선택된 Staff 반환
    /// </summary>
    public Staff GetSelectedStaff() => selectedStaff;

    /// <summary>
    /// Staff가 선택된 상태인지 반환
    /// </summary>
    public bool IsStaffSelected() => isStaffSelected;
}
