using UnityEngine;
using UnityEngine.EventSystems; // We need this to use the IPointerEnterHandler and IPointerClickHandler interfaces

/// <summary>
/// A reusable component that plays sound effects for UI interactions like hover and click.
/// Attach this to any UI element with a button or other selectable component.
/// </summary>
public class UISoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio Clips")]
    [Tooltip("The sound to play when the mouse hovers over this element.")]
    [SerializeField] private AudioClip hoverSound;

    [Tooltip("The sound to play when this element is clicked.")]
    [SerializeField] private AudioClip clickSound;

    /// <summary>
    /// Called by the Event System when the mouse pointer enters the bounds of this UI element.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play the hover sound using our SoundEffectManager
        if (hoverSound != null && SoundEffectManager.instance != null)
        {
            SoundEffectManager.instance.PlaySoundEffect(hoverSound);
        }
    }

    /// <summary>
    /// Called by the Event System when the mouse clicks on this UI element.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Play the click sound using our SoundEffectManager
        if (clickSound != null && SoundEffectManager.instance != null)
        {
            SoundEffectManager.instance.PlaySoundEffect(clickSound);
        }
    }
}