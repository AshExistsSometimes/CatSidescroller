using UnityEngine;
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<string, Action<object>> eventTable = new Dictionary<string, Action<object>>();

    // Subscribes a callback to a named event
    public static void Subscribe(string eventName, Action<object> listener)
    {
        if (!eventTable.ContainsKey(eventName))
            eventTable[eventName] = delegate { };

        eventTable[eventName] += listener;
    }

    // Unsubscribes a callback from a named event
    public static void Unsubscribe(string eventName, Action<object> listener)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName] -= listener;
    }

    // Broadcasts an event with optional data
    public static void Publish(string eventName, object data = null)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable[eventName]?.Invoke(data);
    }
}

// Common event name constants for global consistency
public static class GameEvent
{
    public const string EnemyDefeated = "EnemyDefeated";
    public const string PlayerDamaged = "PlayerDamaged";
    public const string LevelCompleted = "LevelCompleted";
    public const string PlayerDied = "PlayerDied";
}