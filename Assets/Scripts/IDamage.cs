using UnityEngine;

/// <summary>
/// An interface for any game object that can take damage.
/// </summary>
public interface IDamage
{
    /// <summary>
    /// The property to get the object's current health.
    /// </summary>
    int Health { get; set; }

    /// <summary>
    /// The method called to apply damage to the object.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply.</param>
    void TakeDamage(int damageAmount);
}
