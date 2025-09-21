using UnityEngine;

/// <summary>
/// Manages the behavior of a projectile, including movement and collision.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which the projectile travels.")]
    [SerializeField] private float speed = 10f;
    [Tooltip("The amount of damage this projectile deals on impact.")]
    [SerializeField] private int damage = 10;
    [Tooltip("How long the projectile will exist in the world before being destroyed, to prevent clutter.")]
    [SerializeField] private float lifetime = 5f;

    [Header("Effects")]
    [Tooltip("The particle effect to spawn on impact. (Optional)")]
    [SerializeField] private GameObject hitEffect;

    private void Start()
    {
        // Destroy the projectile after its lifetime expires to clean up the scene.
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the projectile forward each frame.
        // 'transform.right' assumes your projectile sprite is pointing to the right.
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Called by Unity's physics engine when this projectile's collider hits another.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to find a damageable component on the object we hit.
        IDamage damageable = other.GetComponent<IDamage>();

        // If the object is damageable, deal damage to it.
        if (damageable != null)
        {
            Debug.Log("Projectile hit " + other.name + ", dealing " + damage + " damage.");
            damageable.TakeDamage(damage);
        }

        // Spawn a hit effect if one is assigned.
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Destroy the projectile on impact.
        Destroy(gameObject);
    }
}