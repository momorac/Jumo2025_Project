using UnityEngine;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private Transform windowLayer;
    [SerializeField] private Transform popupLayer;

    [Header("Window Prefabs")]
    [SerializeField] private HUDView hudView;
    [SerializeField] private PlacementView placementPrefab;

    private WindowType currentOpenWindow;


    private readonly Dictionary<WindowType, WindowViewBase> viewCache = new();
    private readonly Dictionary<WindowType, IPresenter> presenters = new();

    private (HUDView, HUDPresenter) hudPresenters;


    void Start()
    {
        ShowHUD();
    }

    public void OpenWindow(WindowType window)
    {
        // Close current window if any
        if (currentOpenWindow != WindowType.None && currentOpenWindow != window)
        {
            CloseWindow(currentOpenWindow);
        }

        // Open requested window
        var view = GetOrCreateView(window);
        view.Show();

        if (!presenters.ContainsKey(window))
        {
            presenters[window] = CreatePresenter(window, view);
        }

        currentOpenWindow = window;
    }

    public void CloseWindow(WindowType window)
    {
        if (presenters.TryGetValue(window, out var _presenter))
        {
            _presenter.Dispose();
            presenters.Remove(window);
        }

        if (viewCache.TryGetValue(window, out var view))
        {
            view.Hide();
        }

        if (currentOpenWindow == window)
        {
            currentOpenWindow = WindowType.None;
        }
    }

    public void ShowHUD()
    {
        hudPresenters.Item1 = hudView;
        hudPresenters.Item2 = new HUDPresenter(hudView, this);
        hudView.Show();
    }

    public void HideHUD()
    {
        hudView.Hide();
    }

    private WindowViewBase GetOrCreateView(WindowType type)
    {
        if (viewCache.TryGetValue(type, out var existing))
            return existing;

        WindowViewBase created = type switch
        {
            WindowType.Placement => Instantiate(placementPrefab, windowLayer),
            _ => throw new System.NotImplementedException(),
        };

        created.Hide(); // 생성 직후 숨김
        viewCache[type] = created;

        return created;
    }

    private IPresenter CreatePresenter(WindowType type, WindowViewBase view)
    {
        return type switch
        {
            WindowType.Placement => new PlacementPresenter((PlacementView)view, this),
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public enum WindowType
{
    None,
    Placement,
    PauseMenu,
    Settings,
}
