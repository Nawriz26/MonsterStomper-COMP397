using UnityEngine;

/// <summary>
/// Factory Method implementation for collectible creation.
/// Instantiates a collectible prefab and applies CollectibleConfig stats.
/// </summary>
public class CollectibleFactory : MonoBehaviour, IGameObjectFactory
{
    [Header("Prefab")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Configuration")]
    [SerializeField] private CollectibleConfig config;

    /// <summary>Creates a configured collectible at the given position and rotation.</summary>
    public GameObject Create(Vector3 position, Quaternion rotation)
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("CollectibleFactory: collectiblePrefab is not assigned.");
            return null;
        }

        GameObject item = Instantiate(collectiblePrefab, position, rotation);

        if (config != null)
        {
            Collectible collectible = item.GetComponent<Collectible>();
            if (collectible != null)
                collectible.Configure(config);
        }

        return item;
    }
}
