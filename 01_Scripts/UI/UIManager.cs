using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public sealed class UIServices
{
    private readonly Dictionary<Type, object> map = new();
    public void Add<T>(T instance) where T : class => map[typeof(T)] = instance;
    public T Get<T>() where T : class => map.TryGetValue(typeof(T), out var o) ? (T)o : null;
}

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

    // 등록형 팩토리
    private readonly Dictionary<WindowType, Func<WindowViewBase>> viewFactories = new();
    private readonly Dictionary<WindowType, Func<WindowViewBase, IPresenter>> presenterFactories = new();

    private UIServices services;

    void Awake()
    {
        gameSessionRunner = FindFirstObjectByType<GameSessionRunner>();

        // 서비스 등록
        services = new UIServices();
        services.Add(this);
        services.Add(gameSessionRunner);
        services.Add(gameSessionRunner?.PlacementSystem);

        RegisterUI(); // 뷰/프리젠터 생성 람다 등록
    }

    void Start()
    {
        ShowHUD();
    }

    private void RegisterUI()
    {
        // View 생성 람다 등록
        viewFactories[WindowType.Placement] = () => Instantiate(placementPrefab, windowLayer);

        // Presenter 생성 람다 등록 (필요 서비스만 꺼내 주입)
        presenterFactories[WindowType.Placement] = (view) => new PlacementPresenter((PlacementView)view, this, services.Get<PlacementSystem>());
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
        // hudPresenters.Item2?.Dispose(); // 필요 시 이벤트 해제
        hudView.Hide();
    }

    private WindowViewBase GetOrCreateView(WindowType type)
    {
        if (viewCache.TryGetValue(type, out var existing))
            return existing;

        if (!viewFactories.TryGetValue(type, out var factory) || factory == null)
            throw new NotSupportedException($"View factory not registered for {type}");

        var created = factory.Invoke();
        created.Hide(); // 생성 직후 숨김
        viewCache[type] = created;
        return created;
    }

    private IPresenter CreatePresenter(WindowType type, WindowViewBase view)
    {
        if (!presenterFactories.TryGetValue(type, out var factory) || factory == null)
            throw new NotSupportedException($"Presenter factory not registered for {type}");

        return factory.Invoke(view);
    }
}

public enum WindowType
{
    None,
    Placement,
    PauseMenu,
    Settings,
}
