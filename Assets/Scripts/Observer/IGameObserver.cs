/// <summary>
/// Observer interface. Any system that needs to react to game events
/// (Quest, Achievement, UI, etc.) must implement this.
/// </summary>
public interface IGameObserver
{
    /// <summary>Called by the event bus whenever a game event fires.</summary>
    void OnNotify(GameEvent gameEvent, object data);
}
