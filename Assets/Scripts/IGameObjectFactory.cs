using UnityEngine;

/// <summary>
/// Factory Method design pattern interface.
/// All concrete factories implement this to standardise object creation.
/// </summary>
public interface IGameObjectFactory
{
    /// <summary>Creates and returns a fully configured GameObject at the given position and rotation.</summary>
    GameObject Create(Vector3 position, Quaternion rotation);
}
