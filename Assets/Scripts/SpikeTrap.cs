using UnityEngine;

/// <summary>
/// Deals damage to any IDamage-compatible object that touches it.
/// </summary>
public class SpikeTrap : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The amount of damage to deal on contact.")]
    [SerializeField] private int damageAmount = 10;

    // This function is called when another collider enters this object's trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit has a component that can be damaged.
        IDamage damageable = other.GetComponent<IDamage>();

        // If it's a damageable object (like the player)...
        if (damageable != null)
        {
            // ...deal damage to it.
            damageable.TakeDamage(damageAmount);
        }
    }
}