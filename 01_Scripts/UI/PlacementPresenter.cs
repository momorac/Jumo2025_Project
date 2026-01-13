
public class PlacementPresenter : IPresenter
{
    private readonly PlacementView view;
    private readonly UIManager ui;

    private readonly WindowType windowType = WindowType.Placement;

    public PlacementPresenter(PlacementView view, UIManager ui)
    {
        this.view = view;
        this.ui = ui;

        Initialize();
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
