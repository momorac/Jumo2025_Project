using UnityEngine;
using UnityEngine.UI;

/// <summary> Customer 머리 위 말풍선 UI. UI 기반으로 클릭 시 주문 이벤트 발행</summary>
public class BubbleUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button_bubble;
    [SerializeField] private RectTransform rt_bubble;

    private Customer customer;
    private Camera mainCamera;

    private bool isVisible;
    public bool IsVisible => isVisible;
    public bool IsClickable => isVisible && customer != null;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (rt_bubble == null)
        {
            Debug.LogError("rt_bubble reference is missing!");
        }

        SetVisible(false);
        button_bubble.onClick.AddListener(OnBubbleClicked);
    }

    private void LateUpdate()
    {
        if (!isVisible || mainCamera == null)
            return;

        // Billboard: Canvas가 항상 카메라를 바라보도록 회전
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up
        );
    }

    /// <summary>UI 클릭 시 주문 요청 이벤트 발행</summary>
    public void OnBubbleClicked()
    {
        if (!IsClickable)
            return;

        GameLogger.Log(LogCategory.Input, $"Bubble clicked: {customer.name}");

        // BubbleClickedEvent 발행 → TaskAssigner가 처리
        App.EventBus.Publish(new BubbleClickedEvent(customer, customer.transform));
    }


    /// <summary>말풍선 표시/숨김</summary>
    public void SetVisible(bool value)
    {
        isVisible = value;
        rt_bubble.gameObject.SetActive(value);

        button_bubble.interactable = value;
    }

}
