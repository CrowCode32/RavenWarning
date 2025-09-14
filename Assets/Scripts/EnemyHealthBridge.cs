using UnityEngine;

/// <summary>
/// Acts as a bridge between the universal IDamage interface and the specific
/// health system located in the enemyAI script.
/// </summary>
public class EnemyHealthBridge : MonoBehaviour, IDamage
{
    [Header("References")]
    [Tooltip("A reference to the enemy's main AI script.")]
    [SerializeField] private enemyAI enemyController;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    // The 'Health' property required by the IDamage interface.
    public int Health
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// This is the required method from the IDamage interface.
    /// It receives the standardized damage call from the player's attack.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        // Update the health value on this bridge.
        currentHealth -= damageAmount;

        // Pass the damage call along to the enemyAI script, which will
        // handle its own internal health, hit flashes, and death logic.
        if (enemyController != null)
        {
            enemyController.takeDamage(damageAmount);
        }
    }
}