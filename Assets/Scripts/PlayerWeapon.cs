using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private int damage = 25;

    [Header("Raycast Weapon")]
    [SerializeField] private bool useRaycast = false;
    [SerializeField] private float raycastRange = 100f;
    [SerializeField] private LayerMask hitLayers;

    private float nextFireTime = 0f;

    void Start()
    {
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 1f, 1f);
            firePoint = firePointObj.transform;
        }
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        if (useRaycast)
        {
            FireRaycast();
        }
        else
        {
            FireProjectile();
        }

        nextFireTime = Time.time + fireRate;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShoot();
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab not assigned!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(damage);
        }

        Destroy(projectile, 5f);
    }

    private void FireRaycast()
    {
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastRange, hitLayers))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}
