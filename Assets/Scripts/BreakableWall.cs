using UnityEngine;

/// <summary>
/// A simple health script for a wall that can be destroyed.
/// It uses the IDamage interface so any damage source can interact with it.
/// </summary>
public class BreakableWall : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
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
    /// Called when the wall is hit by a damage source.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        Debug.Log("Wall has been destroyed.");
        // We could play a crumbling animation or sound effect here.
        Destroy(gameObject);
    }
}