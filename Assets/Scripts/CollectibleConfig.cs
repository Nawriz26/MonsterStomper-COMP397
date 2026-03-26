using UnityEngine;

/// <summary>ScriptableObject holding all configurable stats for a collectible type.</summary>
[CreateAssetMenu(fileName = "CollectibleConfig", menuName = "MonsterStomper/Factories/Collectible Config")]
public class CollectibleConfig : ScriptableObject
{
    [Header("Identity")]
    public string collectibleName = "Coin";
    public CollectibleType type = CollectibleType.Coin;

    [Header("Value")]
    public int scoreValue = 10;
    public int healAmount = 25;
}
