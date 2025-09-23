using UnityEngine;
using UnityEngine.EventSystems; 

/// <summary>
/// Manages the initial state and selection for a UI menu panel.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    [Tooltip("The first button that should be selected when this menu opens.")]
    [SerializeField] private GameObject firstSelectedButton;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Find the current EventSystem in the scene
        EventSystem eventSystem = EventSystem.current;

        // If the current EventSystem is not found (which causes the error),
        // try to find any EventSystem object in the scene.
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }

        // If an EventSystem is found, set the selected button.
        if (eventSystem != null && firstSelectedButton != null)
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
        else
        {
            Debug.LogWarning("MenuController could not find an EventSystem in the scene.");
        }
    }
}