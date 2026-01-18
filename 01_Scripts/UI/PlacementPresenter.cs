public class PlacementPresenter : IPresenter
{
    private readonly PlacementView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Placement;

    private readonly IPlacementController placementController;

    public PlacementPresenter(PlacementView view, UIManager ui, IPlacementController placementController)
    {
        this.view = view;
        this.ui = ui;
        this.placementController = placementController;
    }

    public void Initialize()
    {
        view.CloseClicked += OnCloseClicked;
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }

    public void Dispose()
    {
        view.CloseClicked -= OnCloseClicked;
    }
}
