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
        view.PlaceClicked += OnPlaceClicked;
        App.EconomyService.OnMoneyChanged += OnMoneyChanged;
        view.UpdateMoney(App.EconomyService.GetMoney());
    }

    private void OnMoneyChanged(int newAmount)
    {
        // 구현
        view.UpdateMoney(newAmount);
    }

    private void OnPlaceClicked()
    {
        // 구현
        ui.OpenWindow(WindowType.Placement);
    }

    public void Dispose()
    {
        view.PlaceClicked -= OnPlaceClicked;
    }
}
