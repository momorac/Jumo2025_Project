public class PlacementPresenter : IPresenter
{
    private readonly PlacementView view;
    private readonly UIManager ui;
    private readonly PlacementSystem placementSystem;

    private readonly WindowType windowType = WindowType.Placement;

    public PlacementPresenter(PlacementView view, UIManager ui, PlacementSystem placementSystem)
    {
        this.view = view;
        this.ui = ui;
        this.placementSystem = placementSystem;
        // Initialize는 외부에서 호출
    }

    public void Initialize()
    {
        view.CloseClicked += OnCloseClicked;
        // placementSystem 사용 로직이 있다면 여기서 구독/초기화
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }

    public void Dispose()
    {
        view.CloseClicked -= OnCloseClicked;
        // placementSystem 구독 해제 등
    }
}
