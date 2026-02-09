using UnityEngine;

/// <summary>
/// Customer 머리 위 말풍선 UI
/// IClickable 구현으로 클릭 시 주문 이벤트 발행
/// </summary>
public class BubbleUI : MonoBehaviour, IClickable
{
    [Header("Components")]
    [SerializeField] private GameObject bubbleObject;
    [SerializeField] private SpriteRenderer bubbleSprite;
    [SerializeField] private Collider bubbleCollider;

    [Header("Settings")]
    [SerializeField] private int clickPriority = 10;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private bool billboardToCamera = true;

    private Customer customer;
    private bool isVisible;
    private Camera mainCamera;

    // IClickable 구현
    public bool IsClickable => isVisible && customer != null;
    public int ClickPriority => clickPriority;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }

        if (bubbleCollider != null)
        {
            bubbleCollider.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (!isVisible || bubbleObject == null)
            return;

        // 위치 업데이트
        if (customer != null)
        {
            bubbleObject.transform.position = customer.transform.position + offset;
        }

        // 카메라를 향해 회전 (Billboard)
        if (billboardToCamera && mainCamera != null)
        {
            bubbleObject.transform.LookAt(
                bubbleObject.transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up
            );
        }
    }

    /// <summary>
    /// 클릭 시 주문 요청 이벤트 발행
    /// </summary>
    public void OnClicked(Vector3 hitPoint)
    {
        if (!IsClickable)
            return;

        Debug.Log($"<color=magenta>Bubble clicked for {customer.name}</color>");

        // BubbleClickedEvent 발행 → TaskAssigner가 처리
        App.EventBus.Publish(new BubbleClickedEvent(customer, customer.transform));
    }

    /// <summary>
    /// Customer 설정
    /// </summary>
    public void SetCustomer(Customer newCustomer)
    {
        customer = newCustomer;
    }

    /// <summary>
    /// 말풍선 표시/숨김
    /// </summary>
    public void SetVisible(bool visible)
    {
        isVisible = visible;

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(visible);
        }

        if (bubbleCollider != null)
        {
            bubbleCollider.enabled = visible;
        }
    }

    /// <summary>
    /// 현재 표시 상태 반환
    /// </summary>
    public bool IsVisible => isVisible;
}
