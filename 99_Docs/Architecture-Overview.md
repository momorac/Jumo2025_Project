# Jumo2025 — 아키텍처 문서

> 최종 업데이트: 2026-04-02
> 스크립트 루트: `Assets/_Project/01_Scripts/`

---

## Index

- [레이어 구조](#레이어-구조)
- [진입점 & 초기화](#진입점--초기화)
  - [GameSessionRunner](#gamesessionrunner)
  - [App (전역 서비스 로케이터)](#app-전역-서비스-로케이터)
- [코어 시뮬레이션](#코어-시뮬레이션)
  - [SimClock](#simclock)
  - [SimLoop](#simloop)
- [페이즈 시스템](#페이즈-시스템)
  - [PhaseController](#phasecontroller)
  - [페이즈 종류](#페이즈-종류)
- [이벤트 시스템](#이벤트-시스템)
  - [GameEventBus](#gameeventbus)
  - [정의된 이벤트 목록](#정의된-이벤트-목록)
- [세이브 시스템](#세이브-시스템)
  - [SaveManager](#savemanager)
  - [InitialSaveConfig](#initialsaveconfig)
  - [GameMetaData](#gamemetadata)
- [Agent 시스템](#agent-시스템)
  - [Staff](#staff)
    - [구조](#구조)
    - [StaffRegistry](#staffregistry)
    - [Staff FSM 상태](#staff-fsm-상태)
    - [Task 시스템](#task-시스템)
  - [Customer](#customer)
    - [구조](#구조-1)
    - [Customer FSM 상태](#customer-fsm-상태)
- [시설 시스템](#시설-시스템)
  - [Placeable 도메인](#placeable-도메인)
  - [조리 시설](#조리-시설)
  - [자원 시설](#자원-시설)
  - [특수 시설](#특수-시설)
- [배치 시스템](#배치-시스템)
  - [GridSystem](#gridsystem)
  - [PlacementSystem](#placementsystem)
  - [PlacementController](#placementcontroller)
  - [PlacementService](#placementservice)
- [경제 시스템](#경제-시스템)
- [식재료 시스템](#식재료-시스템)
- [레시피 시스템](#레시피-시스템)
- [세션 시스템](#세션-시스템)
- [주문 시스템](#주문-시스템)
- [풀링 시스템](#풀링-시스템)
- [클릭 & 포인팅 시스템](#클릭--포인팅-시스템)
- [UI 시스템](#ui-시스템)
- [인프라](#인프라)

---

# 레이어 구조

```
┌─────────────────────────────────────────────────────┐
│  진입점 (Entry)                                      │
│  GameSessionRunner (MonoBehaviour)                   │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│  전역 서비스 로케이터 (App)                           │
│  모든 서비스/레지스트리에 대한 정적 접근 포인트         │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│  Application Layer (서비스)                          │
│  EconomyService / SessionService / PlaceableService  │
│  PlacementService / IngredientService / RecipeService│
│  OrderService / PoolService / TaskAssigner           │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│  Domain Layer (Meta/Data 클래스)                     │
│  EconomyMeta / PlaceableMeta / PlacementMeta         │
│  IngredientMeta / RecipeMeta / SessionMeta           │
└───────────────────┬─────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────┐
│  Persistence Layer                                   │
│  SaveManager / InitialSaveConfig → GameMetaData      │
└─────────────────────────────────────────────────────┘
```

**핵심 원칙**
- 런타임 데이터 접근은 반드시 **서비스를 통해서만** 수행
- 외부(MonoBehaviour, Task 등)는 `App.서비스명.메서드()` 패턴으로 접근
- 저장 대상 데이터는 `*Meta` 클래스로 통일, 서비스 내 `GetMeta()`로 참조 전달

---

# 진입점 & 초기화

## GameSessionRunner

**위치:** `App/GameSessionRunner.cs` | `MonoBehaviour`

게임 씬의 최상위 진입점. `Awake()`에서 코루틴으로 비동기 초기화 순서를 제어한다.

### 초기화 시퀀스

```
Awake()
  └─ InitializeGameSessionCoroutine()
       ├─ App.InitializeGameData(SaveManager.Load(initialSaveConfig))
       │    ├─ SaveManager: 로컬 파일 로드 or InitialSaveConfig로 신규 생성
       │    ├─ 코어 서비스 동기 초기화 (EconomyService, SessionService, PlaceableService ...)
       │    └─ Addressables 비동기 로드 시작 (PoolRegistry, IngredientRegistry, RecipeRegistry)
       │
       ├─ WaitUntil(() => App.HasInitialized)   ← RecipeRegistry 마지막 로드 완료 시 true
       │
       ├─ SimClock / SimLoop 생성
       ├─ CustomerSpawnSimSystem / PedestrianSpawnSimSystem 등록
       ├─ PhaseController 생성 (4개 페이즈 등록, startingPhase로 시작)
       ├─ PlacementController.Initialize()
       └─ UIManager.InjectSessionControllers(placementController)
```

### 매 프레임 루프 (Update)

```
phaseController.Tick(deltaTime)   // 현재 페이즈 로직 실행
simLoop.Update(deltaTime)         // 시뮬레이션 틱 (Open 페이즈 시에만 동작)
IsDayOver() → ChangePhase(Closing)   // 하루 종료 감지
```

### 저장

- 씬 종료(`OnApplicationQuit`) 시 `SaveManager.Save(App.GetSessionDataToMeta())` 자동 호출
- `GetSessionDataToMeta()`는 각 서비스의 `GetMeta()`를 호출해 `GameMetaData` 조립

---

## App (전역 서비스 로케이터)

**위치:** `App/App.cs` | `static class`

모든 서비스와 인프라 객체에 대한 단일 접근 포인트.

```csharp
// 사용 패턴
App.EconomyService.AddIncome(order.Price);
App.TaskQueue.Enqueue(new TakeOrderTask(...));
App.EventBus.Publish(new OrderTakenEvent(...));
App.PlaceableService.GetUnlockedFacilities();
```

### 보유 서비스 목록

| 프로퍼티 | 타입 | 역할 |
|---|---|---|
| `EventBus` | `GameEventBus` | Pub/Sub 이벤트 허브 |
| `TaskQueue` | `TaskQueue` | Staff 작업 대기열 |
| `TaskAssigner` | `TaskAssigner` | 작업 자동/수동 배정 |
| `StaffRegistry` | `StaffRegistry` | 활성 Staff 목록 |
| `Anchors` | `GameAnchors` | 월드 Transform 앵커 모음 |
| `SessionService` | `SessionService` | 좌석/세션 관리 |
| `EconomyService` | `EconomyService` | 수익/지출 처리 |
| `PlaceableService` | `PlaceableService` | 해금 시설/타일 관리 + 런타임 시설 인스턴스 |
| `PlacementService` | `PlacementService` | 배치 데이터(그리드 상태) 관리 |
| `OrderService` | `OrderService` | 주문 생성 및 상태 관리 |
| `PoolService` | `PoolService` | Customer/Pedestrian 오브젝트 풀 |
| `IngredientService` | `IngredientService` | 재료 해금/보유량/구매 |
| `RecipeService` | `RecipeService` | 레시피 해금/조리/버퍼 관리 |

---

# 코어 시뮬레이션

## SimClock

**위치:** `App/SimClock.cs`

하루의 시간 흐름을 `0 ~ 1` 범위의 `Time01`로 표현. `Tick(deltaTime)`으로 진행, `IsDayOver()`가 `true`가 되면 GameSessionRunner가 Closing 페이즈로 전환.

```csharp
DayLengthSeconds = 60f   // 현실 60초 = 게임 하루
Time01 += deltaTime / DayLengthSeconds
```

## SimLoop

**위치:** `App/SimLoop.cs`

고정 0.2초 Tick 기반 시뮬레이션 루프. `isEnabled` 플래그로 활성 제어(Open 페이즈에서만 동작).

### ISimSystem

`Initialize()` + `Tick(float deltaTime)` 인터페이스. 현재 등록된 시스템:

| 시스템 | 역할 |
|---|---|
| `CustomerSpawnSimSystem` | 빈 좌석 체크 후 Customer 스폰 |
| `PedestrianSpawnSimSystem` | 배경용 행인 스폰 |

---

# 페이즈 시스템

## PhaseController

**위치:** `Features/Phase/Application/PhaseController.cs`

4개 페이즈를 `Dictionary<PhaseId, IPhaseState>`로 관리. `Change(PhaseId)`로 전환 시 이전 페이즈 `Exit()` → 새 페이즈 `Enter()`.

## 페이즈 종류

| PhaseId | 클래스 | Enter 동작 | Exit 동작 |
|---|---|---|---|
| `Preparation` | `PreparationPhase` | SimLoop 비활성 | - |
| `Open` | `OpenPhase` | `SimLoop.SetEnabled(true)` | `SimLoop.SetEnabled(false)` |
| `Closing` | `ClosingPhase` | SimLoop 비활성 | - |
| `Upgrade` | `UpgradePhase` | - | - |

페이즈 전환 조건: `Open` 페이즈에서 `simClock.IsDayOver()` → `Closing`으로 자동 전환

---

# 이벤트 시스템

## GameEventBus

**위치:** `App/GameEventBus.cs`

타입 기반 Publish/Subscribe. `IGameEvent` 마커 인터페이스를 구현한 struct를 이벤트 단위로 사용.

```csharp
App.EventBus.Subscribe<CustomerReadyToOrderEvent>(OnReady);
App.EventBus.Publish(new CustomerReadyToOrderEvent(customer, seat, order));
App.EventBus.Unsubscribe<CustomerReadyToOrderEvent>(OnReady);
```

## 정의된 이벤트 목록

**위치:** `App/GameEvents.cs`

| 이벤트 | 발행자 | 구독자 |
|---|---|---|
| `CustomerReadyToOrderEvent` | `CustomerWaitingToOrderState` | `TaskAssigner` |
| `CustomerLeftEvent` | `CustomerLeavingState` | `SessionService` (좌석 해제) |
| `TaskCreatedEvent` | `TaskQueue.Enqueue()` | `TaskAssigner` |
| `TaskAssignedEvent` | `TaskAssigner` | - |
| `OrderTakenEvent` | `TakeOrderTask.OnExecute` | `CustomerController` |
| `OrderServedEvent` | `ServeFoodTask.OnExecute` | `CustomerController` |
| `BubbleClickedEvent` | `BubbleUI` | `TaskAssigner` (TakeOrderTask 생성) |
| `DestinationClickedEvent` | `PointingSystem` | `TaskAssigner` (Staff 이동) |
| `StaffSelectedEvent` | `StaffRegistry` | `StaffCarryingResourceState` |
| `CookingFacilityClickedEvent` | `CookingFacilityBase` | `StaffCarryingResourceState` |

---

# 세이브 시스템

## SaveManager

**위치:** `Features/Save/Application/SaveManager.cs` | `static class`

Newtonsoft.Json으로 `GameMetaData`를 `Application.persistentDataPath/save.json`에 직렬화/역직렬화.

```
Load(InitialSaveConfig)
  ├─ 저장 파일 존재 → Deserialize → GameMetaData
  └─ 저장 파일 없음 → InitializeNewSave(config) → GameMetaData
```

## InitialSaveConfig

**위치:** `Features/Save/Domain/InitialSaveConfig.cs` | `ScriptableObject`

신규 저장 파일 생성 시 초기값 정의. 인스펙터에서 편집.

| 섹션 | 내용 |
|---|---|
| Economy | 시작 골드 |
| Placeable | 초기 해금 시설/타일/장식 목록 |
| Ingredient | 초기 해금 재료 + 재고 |
| Recipe | 초기 해금 레시피 + 버퍼 재고 |

## GameMetaData

**위치:** `App/GameMetaData.cs` | 직렬화 대상

저장 파일과 `App.InitializeGameData()` 사이를 오가는 데이터 컨테이너.

```csharp
public class GameMetaData
{
    public PlaceableMeta  PlaceableMeta;   // 해금 정보
    public PlacementMeta  PlacementMeta;   // 그리드 배치 상태
    public EconomyMeta    EconomyMeta;     // 골드
    public IngredientMeta IngredientMeta;  // 재료 해금 + 재고
    public RecipeMeta     RecipeMeta;      // 레시피 해금 + 버퍼
}
```

> `SessionMeta`(좌석)는 저장 대상이 아님. 매 세션 시작 시 `new SessionMeta()`로 초기화.

---

# Agent 시스템

## Staff

### 구조

Staff는 역할에 따라 3개 컴포넌트로 분리된다.

| 컴포넌트 | 역할 |
|---|---|
| `Staff` | NavMeshAgent 소유, 이동/회전 물리 처리, 외부 퍼블릭 파사드 |
| `StaffController` | FSM 소유, 상태 전환, Prop 관리, Task 진행 |
| `Jumo` | 기본 Staff(주모) 지정용 컴포넌트 (`StaffRegistry.SetDefaultStaff`) |

```
Staff (NavMeshAgent, IClickable)
  └─ StaffController (FSM, Props, Task)
       └─ Jumo (optional: DefaultStaff 지정)
```

### StaffRegistry

씬에 존재하는 모든 Staff를 추적. `Register()`/`Unregister()`는 `StaffController.Start()`/`OnDestroy()`에서 자동 호출.

- `GetIdleStaffs()` : Idle 상태 Staff 목록
- `GetClosestIdleStaff(position)` : 특정 위치 기준 최근접 Idle Staff
- `GetSelectedStaff()` / `SetDefaultStaff()` : 선택/기본 Staff 관리

### Staff FSM 상태

| StateId | 클래스 | 진입 조건 |
|---|---|---|
| `Idle` | `StaffIdleState` | Task 없음, 대기 중 |
| `MovingToTarget` | `StaffMovingToTargetState` | Task Phase의 이동 목표로 이동 중 |
| `ExecutingTask` | `StaffExecutingTaskState` | Task Phase 실행 중 (이동+실행 사이클 내부 처리) |
| `CarryingResource` | `StaffCarryingResourceState` | CollectResourceTask 완료 후 시설로 자원 운반 |
| `Serving` | `StaffServingState` | (구현 중) |

### Task 시스템

#### TaskQueue

우선순위 + 생성 시간 기반 정렬. `Enqueue()` 시 `TaskCreatedEvent` 발행 → `TaskAssigner`가 즉시 배정 시도.

```
pendingTasks  →  Dequeue()  →  assignedTasks
                    ↑
              우선순위 순 정렬
```

#### TaskAssigner

이벤트 구독 기반 자동/수동 배정.

- `TaskCreatedEvent` → `TryAutoAssign()` : Idle Staff에게 자동 배정
- `BubbleClickedEvent` → `TakeOrderTask` 생성 후 최근접 Staff에 배정
- `DestinationClickedEvent` → 선택된(또는 기본) Staff 이동 명령

#### Task 종류

| 타입 | 클래스 | Phase 구성 | 우선순위 |
|---|---|---|---|
| `TakeOrder` | `TakeOrderTask` | 이동→주문 접수 | 10 |
| `ServeFood` | `ServeFoodTask` | 이동→서빙 | 8 |
| `CollectResource` | `CollectResourceTask` | 이동→수집 모션→자원 획득 | 7 |
| `Checkout` | `CheckoutTask` | 이동→계산 처리 (`EconomyService.AddIncome`) | 6 |
| `Cook` | `CookTask` | 이동→조리 모션→`RecipeService.Cook` + 자원 소비 | 5 |
| `ServeD rink` | `ServeDrinkTask` | 이동→음료 서빙 | - |
| `CleanTable` | `CleanTableTask` | 이동→청소 모션 | - |

#### TaskPhase

Task를 구성하는 최소 실행 단위. 선언형으로 비주얼과 비즈니스 로직을 분리.

```csharp
new TaskPhase(
    moveTarget: seatPosition,       // 이동 목표 (null이면 즉시 실행)
    duration: 2f,                   // 실행 시간
    animationTrigger: "TakeOrder",  // 애니메이터 트리거
    propId: StaffPropId.Tray,       // 활성화할 Prop
    onStart: (ctrl) => { ... },     // 실행 시작 콜백
    onExecute: (ctrl) => { ... },   // duration 경과 후 비즈니스 로직
    onEnd: (ctrl) => { ... }        // Phase 종료 콜백
)
```

`StaffExecutingTaskState`가 `Phase 반복 사이클 (Moving → Executing → 다음 Phase)`을 내부에서 완결 처리한다.

---

## Customer

### 구조

| 컴포넌트 | 역할 |
|---|---|
| `Customer` | NavMeshAgent 소유, 이동 처리, `IPooled` 구현 |
| `CustomerController` | FSM 소유, 주문/좌석 데이터 보관, BubbleUI 제어 |
| `BubbleUI` | 말풍선 UI 표시 + 클릭 시 `BubbleClickedEvent` 발행 |

### Customer 수명 주기

```
CustomerSpawnSimSystem.Tick()
  └─ SessionService.TryOccupyRandomSeat()  ← 좌석 선점
       └─ pool.Get() → Customer.SetSeatDelay(seat, delay)
            └─ (지연 후) CustomerController.AssignSeat(seat)
                 └─ FSM 시작: WalkingToSeat
```

```
WalkingToSeat → (도착) → WaitingToOrder → (주문 접수됨) → WaitingForFood
  → (서빙됨) → Eating → (완식) → WaitingForCheckout
  → (계산됨) → Leaving → pool.Release()
                            └─ SessionService 좌석 해제 (CustomerLeftEvent)
```

### Customer FSM 상태

| StateId | 주요 동작 |
|---|---|
| `WalkingToSeat` | NavMesh로 좌석 이동 |
| `WaitingToOrder` | BubbleUI 표시, `CustomerReadyToOrderEvent` 발행, 최대 60초 대기 |
| `WaitingForFood` | `OrderTakenEvent` 수신 후 음식 대기 |
| `Eating` | 식사 모션, 완료 후 WaitingForCheckout |
| `WaitingForCheckout` | 계산 대기 |
| `Leaving` | 퇴장 이동 후 풀 반환 |

---

# 시설 시스템

## Placeable 도메인

**위치:** `Features/Placeable/_Domain/Placeable.cs`

배치 가능한 오브젝트의 타입 계층.

```
Placeable (PlaceableType)
  ├─ Facility (FacilityType)
  ├─ Tile (TileType)
  └─ Decoration (DecorationType)
```

**FacilityType 분류**

| 범위 | 종류 | 설명 |
|---|---|---|
| 특수 | `JumoHouse`, `Table` | 집, 테이블 |
| 100번대 | `Sot`, `Agungi`, `Brazier`, `JangdokJar` | 조리 시설 |
| 200번대 | `Well`, `Stump` | 자원 시설 |

## 조리 시설

**기반 클래스:** `CookingFacilityBase` | `MonoBehaviour, ICookingFacility, IClickable`

```
CookingFacilityBase
  ├─ SotFacility (솥 — 밥/국)
  ├─ AgungiFacility (가마솥 — 반찬/국밥)
  ├─ BrazierFacility (화로 — 전/요리)
  └─ JangdokJarFacility (장독대 — 김치)
```

### 자원 소비 구조

조리 시 물(`currentWater`)과 장작(`currentWood`) 소비. `CanCook = currentWater >= waterPerCook && currentWood >= woodPerCook`.

- `IsWaterNeeded` / `IsWoodNeeded` → `StaffCarryingResourceState`가 감지 후 자원 투입
- `ConsumeResources()` → `CookTask.OnExecute`에서 호출

### 클릭 동작

`SotFacility` 예시: 클릭 시 `CookingFacilityClickedEvent` 발행 + `CookTask`를 `TaskQueue`에 Enqueue.

## 자원 시설

**기반 클래스:** `ResourceFacilityBase` | `MonoBehaviour, IClickable`

```
ResourceFacilityBase
  ├─ WellFacility (우물 — Water)
  └─ StumpFacility (그루터기 — Firewood)
```

- 무한 자원 (`HasResource = true` 항상)
- 클릭 시 최근접 Idle Staff에게 `CollectResourceTask` 배정
- 수집 완료 후 Staff는 `CarryingResourceState`로 전환해 조리 시설로 자원 운반

## 특수 시설

### Table

`IFacility.OnPlaced()` 시 보유한 모든 `Seat`를 `SessionService.RegisterSeat()`에 등록.

---

# 배치 시스템

## GridSystem

**위치:** `Features/Placement/Application/GridSystem.cs` | `MonoBehaviour`

2D 정수 그리드(`PlacementRecord[,]`). `Int2(x, z)` 좌표계 사용.

- `WorldToGrid(Vector3)` / `GridToWorldPivot(Int2)` : 월드↔그리드 변환
- `SetOccupiedRect(root, size, value, placeable)` : 직사각형 영역 일괄 점유
- `CanOccupyRect(root, size)` : 배치 가능 여부 검사
- `GetGridRecords()` / `SetGridRecords(data)` : 세이브/로드

## PlacementSystem

**위치:** `Features/Placement/Application/PlacementSystem.cs` | `MonoBehaviour`

배치 프리뷰 + 실제 배치 + 저장/로드 처리.

- `StartPlacing(placeable, prefab)` → 프리뷰 생성, 마우스 입력 수신
- `Place(cell, prefab)` → Instantiate + `IFacility.Initialize()` + `OnPlaced()` + `SetCellsOccupied()`
- `SavePlacementData()` / `LoadPlacementData()` → `App.PlacementService.UpdateMeta()` / `GetMeta()` 경유

## PlacementController

**위치:** `Features/Placement/Application/PlacementController.cs` | `MonoBehaviour`

UI와 PlacementSystem 사이의 퍼사드. Addressables로 `PlacementRegistry` 로드 후 `PlacementSystem.Initialize(registry)` 호출.

## PlacementService

**위치:** `Features/Placement/Application/PlacementService.cs`

`PlacementMeta` 보관 및 `GetMeta()` / `UpdateMeta()` 제공. PlacementSystem이 배치/로드 시 이 서비스를 통해 Meta를 갱신한다.

---

# 경제 시스템

## EconomyMeta

**위치:** `Features/Economy/Domain/Economy.cs`

골드(`Money`) 보관. `Add(amount)` / `TrySpend(amount)`.  
기본 생성자 보유 (Newtonsoft.Json 역직렬화 대응).

## EconomyService

**위치:** `Features/Economy/Application/EconomyService.cs`

| 메서드 | 동작 |
|---|---|
| `AddIncome(amount)` | 골드 추가 + `OnMoneyChanged` 이벤트 |
| `TrySpend(amount)` | 잔액 충분 시 차감, bool 반환 |
| `CanAfford(amount)` | 잔액 확인 |
| `GetMoney()` | 현재 골드 반환 |
| `GetMeta()` | `EconomyMeta` 참조 반환 (저장용) |

---

# 식재료 시스템

## IngredientMeta

**위치:** `Features/Ingredient/Domain/IngredientData.cs`

```csharp
HashSet<IngredientType> UnlockedIngredients  // 해금된 재료
Dictionary<IngredientType, int> Inventory   // 재료별 보유량
```

## IngredientService

**위치:** `Features/Ingredient/Application/IngredientService.cs`

| 기능 | 메서드 |
|---|---|
| 해금 | `IsUnlocked()`, `Unlock()`, `GetUnlockedIngredients()` |
| 보유량 | `GetAmount()`, `Add()`, `Consume()`, `ConsumeMultiple()`, `HasAmount()` |
| 구매 | `CanPurchase()`, `Purchase()` (EconomyService와 연동) |
| 조회 | `GetDefinition()`, `GetCategory()` |
| 저장 | `GetMeta()` |

**재료 카테고리 (IngredientCategory)**
`Grain(곡식)` / `Vegetable(채소)` / `Meat(육류)` / `Seasoning(부재료)` / `Intermediate(중간재료 — 김치류)`

---

# 레시피 시스템

## RecipeMeta

**위치:** `Features/Recipe/Domain/RecipeData.cs`

```csharp
HashSet<RecipeType> UnlockedRecipes          // 해금된 레시피
Dictionary<RecipeType, int> BufferStock      // 버퍼 자원 재고 (밥/김치)
```

## RecipeService

**위치:** `Features/Recipe/Application/RecipeService.cs`

| 기능 | 메서드 |
|---|---|
| 해금 | `IsUnlocked()`, `Unlock()`, `GetUnlockedRecipes()` |
| 조회 | `GetUnlockedByCategory()`, `GetUnlockedBySubCategory()` |
| 조리 가능 여부 | `CanCook()`, `CanCookUnlocked()`, `GetCookableRecipes()` |
| 조리 실행 | `Cook()` — 재료 소비 + 결과물 처리 |
| 버퍼 | `GetBufferStock()`, `AddToBuffer()`, `ConsumeFromBuffer()` |
| 저장 | `GetMeta()` |

**레시피 분류**

```
RecipeCategory
  ├─ TableDish (차림요리)
  │    ├─ Rice (밥) — WhiteRice, MixedGrainRice, BarleyRice ...
  │    ├─ Soup (국) — RadishSoup, EggSoup, KimchiSoup ...
  │    ├─ SideDish (반찬) — RadishSalad, FriedEgg, Jangjorim ...
  │    └─ Kimchi (김치) — CabbageKimchi, Kkakdugi, Dongchimi ...
  └─ SingleDish (단품요리)
       ├─ StewBowl (국밥/찌개)
       ├─ Jeon (전)
       └─ Dish (요리)
```

조리 시 `IngredientService.ConsumeMultiple()`로 재료 소비. 결과는 중간재료(`outputIngredient`) 또는 버퍼 자원(`isBufferResource`)으로 분기.

---

# 세션 시스템

## SessionMeta

**위치:** `App/GameMetaData.cs`

```csharp
Dictionary<Seat, bool> Seats        // 좌석 → 점유 여부
int AvailableSeatsCount             // 가용 좌석 수 캐시
```

> **저장 대상 아님.** 매 세션 시작 시 `new SessionMeta()`로 초기화.

## SessionService

**위치:** `Features/Session/Application/SessionService.cs`

- `RegisterSeat(seat)` : `Table.OnPlaced()`에서 호출 → 좌석 목록 등록
- `TryOccupyRandomSeat(out seat)` : `CustomerSpawnSimSystem`에서 호출 → 랜덤 빈 좌석 선점
- `OnSeatsChanged` 이벤트 : 좌석 상태 변화 구독 가능

---

# 주문 시스템

## OrderService

**위치:** `Features/Order/OrderService.cs`

현재는 하드코딩 메뉴 목록 사용 (ScriptableObject 교체 예정).

- `CreateRandomOrder()` : 랜덤 메뉴로 `OrderData` 생성
- `UpdateOrderStatus(orderId, status)` : 주문 상태 갱신
- `CompleteOrder(orderId)` : 주문 완료 처리

## OrderData

```csharp
int OrderId
MenuType Type        // Drink / Food / Dessert
string MenuName
int Price
float PrepareTime    // 준비 시간
float EatTime        // 식사 시간
OrderStatus Status   // Created / Taken / Preparing / Served / Completed / Cancelled
```

---

# 풀링 시스템

## PoolService

**위치:** `Features/Pooling/PoolService.cs`

Addressables로 `PoolRegistry` 로드 후 초기화.

| 풀 | 대상 |
|---|---|
| `customerPool` | `Customer` (IPooled) |
| `pedestrianPool` | `Pedestrian` (IPooled) |

`IPooled` 인터페이스: `OnGet()` / `OnRelease()` 콜백.  
Customer는 `pool.Get()` 시 랜덤 스폰 위치 설정, `pool.Release()` 시 풀 반환.

---

# 클릭 & 포인팅 시스템

## PointingSystem

**위치:** `Features/Pointing/PointingSystem.cs` | `MonoBehaviour`

매 프레임 좌클릭 감지. Raycast로 `IClickable` 컴포넌트 탐색(본인→부모 순). UI 위 클릭은 `EventSystem`으로 필터링.

```
좌클릭
  ├─ IClickable 감지 → clickable.OnClicked(hitPoint)
  └─ IClickable 없음 + 땅 레이어 → DestinationClickedEvent 발행
```

## IClickable

```csharp
bool IsClickable { get; }
int ClickPriority { get; }
void OnClicked(Vector3 hitPoint);
```

**구현체:** `Staff`, `CookingFacilityBase`, `WellFacility`, `StumpFacility`, `BubbleUI`

---

# UI 시스템

## UIManager

**위치:** `UI/Framework/UIManager.cs` | `MonoBehaviour`

창 생성/열기/닫기를 팩토리 패턴으로 관리. `WindowType` enum으로 창 구분.

```csharp
// 팩토리 등록 (Awake)
viewFactories[WindowType.Recipe] = () => Instantiate(recipePrefab, windowLayer);
presenterFactories[WindowType.Recipe] = (view) => new RecipePresenter(view, this, App.IngredientService, App.RecipeService);

// 창 열기 (캐싱)
OpenWindow(WindowType.Recipe)  →  view 캐시 확인 → 없으면 팩토리로 생성
```

## MVP 패턴

각 UI 창은 View / Presenter로 분리.

| 창 | View | Presenter |
|---|---|---|
| HUD | `HUDView` | `HUDPresenter` |
| 배치 | `PlacementView` | `PlacementPresenter` |
| 인벤토리 | `InventoryView` | `InventoryPresenter` |
| 레시피북 | `RecipeView` | `RecipePresenter` |

- **View** : `WindowViewBase` 상속, UI 컴포넌트 바인딩 및 갱신만 담당
- **Presenter** : 서비스 레이어 호출, View에 데이터 전달

### HUD

`App.EconomyService.OnMoneyChanged` 이벤트 구독 → 골드 수치 실시간 갱신.

### 인벤토리 창

`IngredientService`에서 해금 재료 목록 조회, 카테고리별 `contentParent`에 `InventoryCellView` 생성.

### 레시피북 창

`RecipeService.GetUnlockedBySubCategory()`로 소분류별 카드 목록 표시.  
각 카드에서 재료 아이콘은 `IngredientService.GetDefinition()`으로 조회.

---

# 인프라

## GameLogger

**위치:** `Debug/GameLogger.cs` | `static class`

카테고리별 색상 + 레벨 필터링 로그 유틸리티.

| `LogLevel` | 용도 |
|---|---|
| `Error` | 항상 출력 |
| `Info` | 주요 이벤트 (배정, 완료, 생성) |
| `Verbose` | 상세 디버그 (상태 전환, 자원 변동) |

| `LogCategory` | 색상 |
|---|---|
| `Staff` | cyan |
| `Customer` | blue |
| `Task` | lime |
| `Facility` | orange |
| `Economy` | yellow |
| `System` | (기본) |
| `Input` | - |
| `UI` | - |

## GameAnchors

**위치:** `App/GameAnchors.cs` | `MonoBehaviour`

씬의 주요 Transform 앵커를 `App.Anchors`에 등록.

```csharp
Transform PedestrianRoot
Transform CustomerRoot
Transform UIRoot
Transform UI_BubbleRoot
Transform[] CustomerSpawnPoints
```


---

## 2. GameSessionRunner — 게임 루프 진입점

**위치:** `App/GameSessionRunner.cs`

```
Awake → InitializeGameSessionCoroutine()
    App.InitializeGameData(SaveManager.Load(...))
    await HasInitialized
    SimClock, SimLoop 생성
    PhaseController 생성 (4개 Phase 주입)
    PlacementController.Initialize()

Update (매 프레임)
    phaseController.Tick(dt)
    simLoop.Update(dt)
    if (OpenPhase && IsDayOver()) → ChangePhase(Closing)
```

---

## 3. 게임 페이즈 시스템

**위치:** `Features/Phase/`

### PhaseId enum
```
Preparation → Open → Closing → Upgrade → (Preparation...)
```

### IPhaseState 인터페이스
```csharp
public interface IPhaseState
{
    PhaseId Id { get; }
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
```

### 각 Phase 역할

| Phase | Enter | Exit | 특이사항 |
|---|---|---|---|
| `PreparationPhase` | SimLoop 비활성 | - | 배치/준비 시간 |
| `OpenPhase` | `simLoop.SetEnabled(true)` | `simLoop.SetEnabled(false)` | 영업 시간. SimClock 진행 |
| `ClosingPhase` | - | - | 마감 연출 |
| `UpgradePhase` | - | - | 업그레이드 선택 |

### PhaseController
- `Dictionary<PhaseId, IPhaseState>` 로 관리
- `Change(PhaseId)` → Exit 현재 → Enter 다음
- `Tick(dt)` → 현재 Phase에 위임

**전환 트리거:** `GameSessionRunner.Update()`에서 `simClock.IsDayOver()` 체크

---

## 4. SimLoop + SimClock — 시뮬레이션 엔진

**위치:** `App/SimLoop.cs`, `App/SimClock.cs`

```
SimClock
    DayLengthSeconds : float    (Inspector: 60f)
    Time01 : float              (0 ~ 1, 하루 진행도)
    Day : int
    IsDayOver() → Time01 >= 1f

SimLoop
    TICK = 0.2f                 (고정 간격)
    accumulatedTime 누적 → Tick 호출
    각 ISimSystem.Tick(0.2f) 순차 실행
    isEnabled = false 이면 전체 정지
```

### ISimSystem 인터페이스
```csharp
public interface ISimSystem
{
    void Initialize();
    void Tick(float deltaTime);   // 0.2s 간격 호출
}
```

**등록된 시스템:**
- `CustomerSpawnSimSystem` — 손님 스폰 로직
- `PedestrianSpawnSimSystem` — 보행자 스폰 로직

> 새 시뮬레이션 로직 추가 시 `ISimSystem` 구현 후 `AddSimSystems()`에 등록.

---

## 5. GameEventBus — 타입 기반 Pub/Sub

**위치:** `App/GameEventBus.cs`

```csharp
// 구독
App.EventBus.Subscribe<CustomerReadyToOrderEvent>(OnCustomerReady);

// 발행
App.EventBus.Publish(new CustomerReadyToOrderEvent(customer, seat, order));

// 해제
App.EventBus.Unsubscribe<CustomerReadyToOrderEvent>(OnCustomerReady);
```

**등록된 이벤트 (GameEvents.cs):**

| 이벤트 | 발행 시점 |
|---|---|
| `CustomerReadyToOrderEvent` | Customer가 착석, 주문 준비됨 |
| `CustomerLeftEvent` | Customer 퇴장 (대기 초과 등) |
| `TaskCreatedEvent` | TaskQueue에 Task 추가 시 |
| `TaskAssignedEvent` | Task가 Staff에게 배정될 때 |
| `TaskCompletedEvent` | Task 완료 시 |
| `OrderTakenEvent` | 주문 접수 완료 |
| `OrderServedEvent` | 주문 서빙 완료 |

---

## 6. Customer FSM

**위치:** `Features/Agent/Customer/`

### 상태 7개

```
Spawned → WalkingToSeat → WaitingToOrder
                               ↓ (OrderTaken)
                          WaitingForFood
                               ↓ (OrderServed)
                            Eating
                               ↓ (식사 완료)
                      WaitingForCheckout
                               ↓ (Checkout 완료)
                            Leaving
```

### ICustomerState 인터페이스
```csharp
public enum CustomerStateId
{
    Spawned, WalkingToSeat, WaitingToOrder,
    WaitingForFood, Eating, WaitingForCheckout, Leaving
}

public interface ICustomerState
{
    CustomerStateId Id { get; }
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
```

**CustomerController** — Staff의 StaffController와 동일한 FSM 패턴.

---

## 7. Staff FSM + Task 시스템

> 상세 내용: [Staff-Task-Architecture.md](Staff-Task-Architecture.md)

### 요약

```
Staff FSM (3 상태)
    Idle → MovingToTarget → ExecutingTask → Idle

Task = List<TaskPhase>
    TaskPhase: MoveTarget + Duration + OnStart + OnExecute + OnEnd

실행 흐름:
    AssignTask → Phase[0] → WillMoveFirst? → MovingToTarget or ExecutingTask
               → PhaseCompleted → Phase[1]... → CompleteTask → Idle
```

### Task 종류 (`Features/Agent/Staff/Task/Tasks/`)

| Task | 설명 | Phase 수 |
|---|---|---|
| `TakeOrderTask` | 손님 테이블 → 주문 접수 | 1 |
| `ServeDrinkTask` | 손님 테이블 → 음료 서빙 | 1 |
| `ServeFoodTask` | 손님 테이블 → 음식 서빙 | 1 |
| `CleanTableTask` | 테이블 이동 → 청소 | 1 |
| `CheckoutTask` | 계산대 → 계산 처리 | 1 |
| `CollectResourceTask` | 시설(우물/장작) → 자원 수집 | 1 |

### TaskQueue 동작

```
Enqueue(task) → SortByPriority → Publish(TaskCreatedEvent)
                                         ↓
                               TaskAssigner 수신
                                         ↓
                          DequeueClosestTo(staff.position)
                                         ↓
                               staff.AssignTask(task)
```

---

## 8. 디렉터리 구조 전체

```
Assets/_Project/01_Scripts/
    App/
        App.cs                  ← 전역 서비스 로케이터
        GameSessionRunner.cs    ← 게임 루프 진입점
        GameEventBus.cs         ← Pub/Sub 이벤트 버스
        GameEvents.cs           ← 이벤트 타입 정의
        SimClock.cs             ← 게임 내 시간
        SimLoop.cs              ← 고정 Tick 시뮬레이션 루프
        ISimSystem.cs           ← 시뮬레이션 시스템 인터페이스
        GameAnchors.cs          ← 씬 참조 앵커
        GameMetaData.cs         ← 저장 데이터 컨테이너

    Features/
        Phase/
            Application/
                PhaseController.cs
            Domain/
                IPhaseState.cs
                PreparationPhase.cs
                OpenPhase.cs
                ClosingPhase.cs
                UpgradePhase.cs

        Session/
            Application/
                SessionService.cs   ← 좌석 관리

        Economy/
            (EconomyService, EconomyData)

        Order/
            OrderData.cs
            OrderService.cs

        Agent/
            Customer/
                Customer.cs
                CustomerController.cs
                States/
                    ICustomerState.cs   ← enum + 인터페이스
                    CustomerWalkingToSeatState.cs
                    CustomerWaitingToOrderState.cs
                    CustomerWaitingForFoodState.cs
                    CustomerEatingState.cs
                    CustomerWaitingForCheckoutState.cs
                    CustomerLeavingState.cs

            Staff/
                Staff.cs
                StaffController.cs
                States/
                    IStaffState.cs      ← enum + 인터페이스
                    StaffIdleState.cs
                    StaffMovingToTargetState.cs
                    StaffExecutingTaskState.cs
                Task/
                    IStaffTask.cs       ← 인터페이스 + TaskType enum
                    TaskPhase.cs        ← Phase 데이터 클래스
                    TaskQueue.cs        ← 우선순위 대기열
                    TaskAssigner.cs     ← 자동 배정
                    StaffRegistry.cs    ← 활성 Staff 목록
                    Tasks/
                        StaffTaskBase.cs
                        TakeOrderTask.cs
                        ServeDrinkTask.cs
                        ServeFoodTask.cs
                        CleanTableTask.cs
                        CheckoutTask.cs
                        CollectResourceTask.cs
```

---

## 9. 핵심 설계 원칙

| 원칙 | 적용 |
|---|---|
| **서비스 로케이터** | `App.XXX`로 전역 접근. DI 대신 단순성 우선 |
| **FSM 패턴 일관성** | Staff, Customer, Phase 모두 동일한 `Enter/Tick/Exit` 구조 |
| **Pub/Sub 이벤트** | Customer ↔ Staff 간 직접 참조 없이 EventBus 경유 |
| **SimLoop 분리** | 게임 로직(0.2s Tick)과 렌더 루프(Update) 분리 |
| **Lazy Init** | Task.Phases는 `_phases ??= BuildPhases()` (base ctor 타이밍 문제 방지) |
| **Task = 데이터** | Task는 Phase 목록을 명세할 뿐, 실행 제어는 StaffController |
| **단일 책임 파일** | Task 클래스 1개 = 파일 1개 분리 |
