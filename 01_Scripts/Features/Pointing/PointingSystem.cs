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
    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        // UI 위를 클릭한 경우 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);


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
            }

            else if (clickable == null)
            {
                if (((1 << hit.collider.gameObject.layer) & groundMask) != 0)
                {
                    App.EventBus.Publish(new DestinationClickedEvent(hit.point, null));
                }
            }
        }
    }
}
