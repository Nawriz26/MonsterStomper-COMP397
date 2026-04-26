/// <summary>
/// Subject (Publisher) interface. Implemented by the GameEventBus.
/// </summary>
public interface IGameSubject
{
    /// <summary>Registers an observer to receive future notifications.</summary>
    void RegisterObserver(IGameObserver observer);

    /// <summary>Unregisters an observer so it no longer receives notifications.</summary>
    void UnregisterObserver(IGameObserver observer);

    /// <summary>Notifies all registered observers with the given event and optional payload.</summary>
    void NotifyObservers(GameEvent gameEvent, object data = null);
}
