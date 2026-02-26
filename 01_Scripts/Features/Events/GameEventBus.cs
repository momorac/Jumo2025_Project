using System;
using System.Collections.Generic;

/// <summary>
/// 타입 기반 Publish/Subscribe 이벤트 버스
/// Customer-Staff 간 느슨한 결합 통신을 위한 중앙 허브
/// </summary>
public class GameEventBus
{
    private readonly Dictionary<Type, List<Delegate>> subscribers = new();

    /// <summary>특정 이벤트 타입을 구독합니다</summary>
    public void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        var type = typeof(T);
        if (!subscribers.ContainsKey(type))
        {
            subscribers[type] = new List<Delegate>();
        }
        subscribers[type].Add(handler);
    }

    /// <summary>특정 이벤트 타입 구독을 해제합니다</summary>
    public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        var type = typeof(T);
        if (subscribers.ContainsKey(type))
        {
            subscribers[type].Remove(handler);
        }
    }

    /// <summary>이벤트를 발행하여 모든 구독자에게 전달합니다</summary>
    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        var type = typeof(T);
        if (!subscribers.ContainsKey(type))
            return;

        // 리스트 복사하여 순회 중 수정 방지
        var handlers = new List<Delegate>(subscribers[type]);
        foreach (var handler in handlers)
        {
            ((Action<T>)handler)?.Invoke(gameEvent);
        }
    }

    /// <summary>모든 구독을 해제합니다</summary>
    public void Clear()
    {
        subscribers.Clear();
    }

    /// <summary>특정 이벤트 타입의 모든 구독을 해제합니다</summary>
    public void Clear<T>() where T : IGameEvent
    {
        var type = typeof(T);
        if (subscribers.ContainsKey(type))
        {
            subscribers[type].Clear();
        }
    }
}
