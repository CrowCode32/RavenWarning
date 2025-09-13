using UnityEngine;

public class heal : MonoBehaviour
{
    [SerializeField] int healAmount;
 private void OnTriggerEnter2D(Collider2D other)
    {
        IHeal heal = other.GetComponent<IHeal>();
        if(other.isTrigger)
        {
            return;
        }
        else
        {
            heal.Heal(healAmount);
        }

    }
}
