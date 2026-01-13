public class HUDPresenter : IPresenter
{
    private readonly HUDView view;
    private readonly UIManager ui;

    public HUDPresenter(HUDView view, UIManager ui)
    {
        this.view = view;
        this.ui = ui;

        Initialize();
    }

    public void Initialize()
    {
        view.PlacementClicked += OnPlacementClicked;

    }
    private void OnPlacementClicked()
    {
        // 구현
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
