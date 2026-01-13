using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private Transform windowLayer;
    [SerializeField] private Transform popupLayer;

    [Header("Window Prefabs")]
    [SerializeField] private HUDView hudView;
    [SerializeField] private PlacementView placementPrefab;

    [HideInInspector] public GameSessionRunner gameSessionRunner;

    // Cached presenters and views
    private (HUDView, HUDPresenter) hudPresenters;
    private WindowType currentOpenWindow;

    private readonly Dictionary<WindowType, WindowViewBase> viewCache = new();
    private readonly Dictionary<WindowType, IPresenter> presenters = new();

    void Awake()
    {
        gameSessionRunner = FindFirstObjectByType<GameSessionRunner>();
    }

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

        var view = GetOrCreateView(window);
        view.Show();

        if (!presenters.ContainsKey(window))
        {
            var p = CreatePresenter(window, view);
            presenters[window] = p;
            p.Initialize(); // 생성 직후 초기화
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
        hudPresenters.Item2.Initialize(); // 생성 직후 초기화
        hudView.Show();
    }

    public void HideHUD()
    {
        // 필요 시 Dispose 호출로 이벤트 해제
        // hudPresenters.Item2?.Dispose();
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
            WindowType.Placement => new PlacementPresenter(
                (PlacementView)view,
                this,
                gameSessionRunner.PlacementSystem),
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
