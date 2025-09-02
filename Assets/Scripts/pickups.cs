using UnityEngine;

public class pickups : MonoBehaviour
{
    [SerializeField] trinket trinket;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Yep thats a trinket");
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null)
        {
            pickupable.getTrinket(trinket);
            Destroy(gameObject);
        }
    }
}
