using UnityEngine;

/// <summary>ScriptableObject holding all configurable stats for a projectile type.</summary>
[CreateAssetMenu(fileName = "ProjectileConfig", menuName = "MonsterStomper/Factories/Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Identity")]
    public string projectileName = "Stone";

    [Header("Combat")]
    public int damage = 25;

    [Header("Physics")]
    public float speed = 20f;
    public float lifetime = 15f;
}
