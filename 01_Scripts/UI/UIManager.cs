using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private Transform windowLayer;
    [SerializeField] private Transform popupLayer;


    [Header("Window Prefabs")]
    [SerializeField] private HUDView hudPrefab;


    private WindowType currentOpenWindow;

    private readonly Dictionary<WindowType, WindowViewBase> viewCache = new();
    private readonly Dictionary<WindowType, IPresenter> presenters = new();


    void Start()
    {
        OpenWindow(WindowType.HUD);
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
        // Implementation for closing the specified window
    }

    private WindowViewBase GetOrCreateView(WindowType type)
    {
        if (viewCache.TryGetValue(type, out var existing))
            return existing;

        WindowViewBase created = type switch
        {
            WindowType.HUD => Instantiate(hudPrefab, windowLayer),
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
            WindowType.HUD => new HUDPresenter((HUDView)view, this),
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public enum WindowType
{
    None,
    HUD,
    PauseMenu,
    Settings,
}
