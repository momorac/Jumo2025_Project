using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Customer FSM 컨트롤러. 상태 전환 및 UI/이벤트 관리
/// 이동 제어는 Customer에 위임
/// </summary>
[RequireComponent(typeof(Customer))]
public class CustomerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private BubbleUI bubbleUI;

    // FSM
    private Dictionary<CustomerStateId, ICustomerState> states;
    private ICustomerState currentState;

    // 데이터
    private Customer customer;
    private Seat assignedSeat;
    private OrderData currentOrder;
    private bool wasServed;

    // 프로퍼티
    public Customer Customer => customer;
    public Seat AssignedSeat => assignedSeat;
    public OrderData CurrentOrder => currentOrder;
    public bool WasServed => wasServed;
    public CustomerStateId CurrentStateId => currentState?.Id ?? CustomerStateId.Spawned;

    private void Awake()
    {
        customer = GetComponent<Customer>();

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
            GameLogger.LogWarning(LogCategory.Customer, $"State {newStateId} not found");
            return;
        }

        currentState?.Exit();
        currentState = states[newStateId];
        currentState.Enter();
    }


    public void AssignSeat(Seat seat)
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
        transform.SetPositionAndRotation(assignedSeat.Root.position, assignedSeat.Root.rotation);
        TriggerAnimation("SitTrigger");

        // 모션 애니메이션에 transform 적용
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(assignedSeat.MotionRoots[0].position, 0.33f)).Join(transform.DOLocalRotateQuaternion(assignedSeat.MotionRoots[0].rotation, 0.33f));
        seq.Append(transform.DOMove(assignedSeat.MotionRoots[1].position, 0.66f)).Join(transform.DOLocalRotateQuaternion(assignedSeat.MotionRoots[1].rotation, 0.66f));
        seq.Append(transform.DOMove(assignedSeat.MotionRoots[2].position, 0.66f)).Join(transform.DOLocalRotateQuaternion(assignedSeat.MotionRoots[2].rotation, 0.66f));
        seq.AppendCallback(() =>
        {
            // 착석 후 주문 대기 상태로 전환
            ChangeState(CustomerStateId.WaitingToOrder);
        });
        seq.Play();


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
        GameLogger.LogWarning(LogCategory.Customer, $"{name}: leaving without being served");
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

    // ── NavMesh 위임 래퍼 (상태 클래스는 controller만 바라봄) ──

    /// <summary>NavMesh 목적지 설정</summary>
    public void SetDestination(Vector3 position) => customer.SetDestination(position);

    /// <summary>이동 정지</summary>
    public void StopMoving() => customer.StopMoving();

    /// <summary>목적지 도착 여부 확인</summary>
    public bool HasReachedDestination() => customer.HasReachedDestination();

    /// <summary>NavMeshAgent 활성화/비활성화</summary>
    public void EnableNavMeshAgent(bool enable) => customer.EnableNavMeshAgent(enable);

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
