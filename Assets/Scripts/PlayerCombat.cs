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

    [Header("Required Components")]
    [Tooltip("Reference to the main player controller script.")]
    [SerializeField] private playerController controller;

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
        }
    }
}