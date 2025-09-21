using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's attack input using the new Input System and triggers
/// the attack logic located in the playerController.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("The Input Action for the player's attack.")]
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] Transform attackPoint;

    [Header("Required Components")]
    [Tooltip("Reference to the main player controller script.")]
    [SerializeField] private playerController controller;

    [Header("Projectile Settings")]
    [Tooltip("The projectile prefab to be fired.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("The position from which the projectile is fired.")]
    [SerializeField] private Transform firePoint;


    // Subscribe to the input action.
    private void OnEnable()
    {
        attackAction.action.performed += OnAttack;
    }

    // Unsubscribe from the input action.
    private void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
    }

    /// <summary>
    /// Called by the Input System when the Attack button is pressed.
    /// </summary>
    private void OnAttack(InputAction.CallbackContext context)
    {
        // Tell the playerController to perform its slashAttack method.
        if (controller != null)
        {
            controller.slashAttack();
            FireProjectile();
        }
    }

  /// <summary>
  /// Spawns a projectile from the fire point.
  /// </summary>
    private void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Create a new projectile at the fire point's position and rotation.
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("PlayerCombat is missing a Projectile Prefab or Fire Point reference!");
        }

    }
}