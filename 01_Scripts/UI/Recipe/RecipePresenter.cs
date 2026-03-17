using UnityEngine;


public class RecipePresenter : IPresenter
{
    private readonly RecipeView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Recipe;

    public RecipePresenter(RecipeView view, UIManager ui)
    {
        this.view = view;
        this.ui = ui;
    }

    public void Initialize()
    {
        view.CloseButtonClicked += OnCloseClicked;
    }

    public void Dispose()
    {
        view.CloseButtonClicked -= OnCloseClicked;
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }
}
