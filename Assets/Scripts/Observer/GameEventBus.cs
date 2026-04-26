using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Publisher (Subject) for the Observer Pattern.
/// All game systems raise events through this bus; Quest and Achievement managers
/// register here as Observers to react without tight coupling.
/// </summary>
public class GameEventBus : MonoBehaviour, IGameSubject
{
    public static GameEventBus Instance { get; private set; }

    private readonly List<IGameObserver> observers = new List<IGameObserver>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Registers an observer to receive all future game event notifications.</summary>
    public void RegisterObserver(IGameObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    /// <summary>Removes an observer so it no longer receives notifications.</summary>
    public void UnregisterObserver(IGameObserver observer)
    {
        observers.Remove(observer);
    }

    /// <summary>
    /// Broadcasts a game event to every registered observer.
    /// Iterate a copy of the list to allow observers to unregister mid-notification.
    /// </summary>
    public void NotifyObservers(GameEvent gameEvent, object data = null)
    {
        List<IGameObserver> snapshot = new List<IGameObserver>(observers);
        foreach (IGameObserver observer in snapshot)
        {
            observer.OnNotify(gameEvent, data);
        }
    }

    /// <summary>Static convenience method for raising events from anywhere in the codebase.</summary>
    public static void Raise(GameEvent gameEvent, object data = null)
    {
        if (Instance != null)
            Instance.NotifyObservers(gameEvent, data);
        else
            Debug.LogWarning($"GameEventBus: No instance found. Cannot raise event {gameEvent}.");
    }
}
