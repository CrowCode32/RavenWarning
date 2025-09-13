using UnityEngine;

/// <summary>
/// Acts as a bridge between the universal IDamage interface and the specific
/// health system located in the playerController script. This bridge also
/// holds the authoritative health value for the combat system.
/// </summary>
public class PlayerHealthBridge : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    [Tooltip("Set the player's maximum health here.")]
    [SerializeField] private int maxHealth = 100;

    [Header("References")]
    [Tooltip("A reference to the player's main controller script.")]
    [SerializeField] private playerController controller;

    private int currentHealth;

    // The 'Health' property is required by the IDamage interface.
    // This now uses the health value stored within this bridge script.
    public int Health
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private void Start()
    {
        // Initialize the health for the combat system.
        currentHealth = maxHealth;
    }

    /// <summary>
    /// This is the required method from the IDamage interface.
    /// It receives the standardized damage call.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        // 1. Update the health value on this bridge.
        currentHealth -= damageAmount;

        // 2. Pass the damage call along to the playerController, which will
        //    handle its own internal health, UI, and death logic.
        if (controller != null)
        {
            controller.takeDamage(damageAmount);
        }
    }
}