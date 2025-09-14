using UnityEngine;

/// <summary>
/// Acts as a simple bridge to make the existing enemyAI script compatible
/// with the IDamage interface without modifying its source code.
/// </summary>
public class EnemyHealthBridge : MonoBehaviour, IDamage
{
    [Header("References")]
    [Tooltip("A reference to the enemy's main AI script.")]
    [SerializeField] private enemyAI enemyController;

    // The 'Health' property is required by the IDamage interface.
    // We can just return a placeholder value since the real health is managed by the enemyAI.
    public int Health
    {
        // We don't have access to the enemy's real health, so we just return 1.
        // This doesn't affect gameplay, it just fulfills the interface contract.
        get { return 1; }
        set { /* We don't need to do anything here. */ }
    }

    /// <summary>
    /// This is the required method from the IDamage interface.
    /// It receives the standardized damage call and passes it on.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        // Pass the damage call directly to the enemyAI script, which will
        // handle all the health logic, hit flashes, and death.
        if (enemyController != null)
        {
            enemyController.takeDamage(damageAmount);
        }
    }
}