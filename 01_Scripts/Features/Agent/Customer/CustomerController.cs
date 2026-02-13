using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Customer FSM 컨트롤러. 상태 전환 및 UI/이벤트 관리
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CustomerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private BubbleUI bubbleUI;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 0.3f;

    // FSM
    private Dictionary<CustomerStateId, ICustomerState> states;
    private ICustomerState currentState;

    // 데이터
    private Customer customer;
    private Transform assignedSeat;
    private OrderData currentOrder;
    private bool wasServed;

    // 프로퍼티
    public Customer Customer => customer;
    public Transform AssignedSeat => assignedSeat;
    public OrderData CurrentOrder => currentOrder;
    public bool WasServed => wasServed;
    public CustomerStateId CurrentStateId => currentState?.Id ?? CustomerStateId.Spawned;

    private void Awake()
    {
        customer = GetComponent<Customer>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        InitializeStates();
    }

    private void Start()
    {
        // 주문 접수 이벤트 구독
        App.EventBus.Subscribe<OrderTakenEvent>(OnOrderTakenEvent);
    }

    private void OnDestroy()
    {
        if (App.EventBus != null)
        {
            App.EventBus.Unsubscribe<OrderTakenEvent>(OnOrderTakenEvent);
        }
    }

    private void OnOrderTakenEvent(OrderTakenEvent evt)
    {
        // 자신의 주문인 경우에만 처리
        if (evt.Customer == customer)
        {
            OnOrderTaken();
        }
    }

    private void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    private void InitializeStates()
    {
        states = new Dictionary<CustomerStateId, ICustomerState>
        {
            { CustomerStateId.WalkingToSeat, new CustomerWalkingToSeatState(this) },
            { CustomerStateId.WaitingToOrder, new CustomerWaitingToOrderState(this) },
            { CustomerStateId.WaitingForFood, new CustomerWaitingForFoodState(this) },
            { CustomerStateId.Eating, new CustomerEatingState(this) },
            { CustomerStateId.WaitingForCheckout, new CustomerWaitingForCheckoutState(this) },
            { CustomerStateId.Leaving, new CustomerLeavingState(this) }
        };
    }

    public void ChangeState(CustomerStateId newStateId)
    {
        if (!states.ContainsKey(newStateId))
        {
            Debug.LogWarning($"Customer state {newStateId} not found!");
            return;
        }

        currentState?.Exit();
        currentState = states[newStateId];
        currentState.Enter();
    }


    public void AssignSeat(Transform seat)
    {
        assignedSeat = seat;

        // 주문 생성
        currentOrder = App.OrderService.CreateRandomOrder();

        // 이동 시작
        ChangeState(CustomerStateId.WalkingToSeat);
    }

    /// <summary> 착석 처리 </summary>
    public void SitDown()
    {
        EnableNavMeshAgent(false);

        // 좌석 위치로 정확히 이동
        transform.SetPositionAndRotation(assignedSeat.position, assignedSeat.rotation);
        TriggerAnimation("SitTrigger");

        // 주문 대기 상태로 전환
        ChangeState(CustomerStateId.WaitingToOrder);
    }

    /// <summary> 주문 접수됨 (Staff가 주문을 받음) </summary>
    public void OnOrderTaken()
    {
        wasServed = true;
        currentOrder.Status = OrderStatus.Ordered;

        // 음식 대기 상태로 전환
        ChangeState(CustomerStateId.WaitingForFood);
    }

    /// <summary>주문 없이 이탈</summary>
    public void LeaveWithoutOrder()
    {
        wasServed = false;
        Debug.Log($"<color=red>{name}: Leaving without being served!</color>");
        ChangeState(CustomerStateId.Leaving);
    }

    /// <summary> 정상 퇴장 </summary>
    public void Leave()
    {
        // 주문 완료 처리
        if (currentOrder != null)
        {
            App.OrderService.CompleteOrder(currentOrder.OrderId);
        }

        ChangeState(CustomerStateId.Leaving);
    }

    /// <summary> 좌석 해제 </summary>
    public void ReleaseSeat()
    {
        if (assignedSeat != null)
        {
            // SessionService에 좌석 반환 로직 필요 시 추가
            assignedSeat = null;
        }
    }

    /// <summary> 풀에 반환 </summary>
    public void ReturnToPool()
    {
        customer.Release();
    }

    /// <summary>말풍선 표시/숨김</summary>
    public void ShowBubble(bool value)
    {
        bubbleUI.SetVisible(value);
    }

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position)
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(position);
        }
    }

    /// <summary>NavMeshAgent 활성화/비활성화</summary>
    public void EnableNavMeshAgent(bool enable)
    {
        if (agent != null)
        {
            agent.enabled = enable;
        }
    }

    /// <summary>목적지 도착 여부 확인</summary>
    public bool HasReachedDestination()
    {
        if (agent == null || !agent.enabled)
            return true;

        return !agent.pathPending && agent.remainingDistance <= stoppingDistance;
    }

    /// <summary>애니메이션 파라미터 설정</summary>
    public void SetAnimation(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }

    /// <summary>애니메이션 트리거 설정</summary>
    public void TriggerAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
