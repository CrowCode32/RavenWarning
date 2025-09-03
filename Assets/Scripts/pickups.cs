using UnityEngine;

public class pickups : MonoBehaviour
{
    [SerializeField] trinket trinket;
    [SerializeField] feather feather;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null && trinket != null)
        {
            Debug.Log("Yep thats a trinket");
            pickupable.getTrinket(trinket);
            Destroy(gameObject);
        }
        else if(pickupable != null && feather != null)
        {
            Debug.Log("Nah thats a feather");
            pickupable.getFeather(feather);
            Destroy(gameObject);
        }
    }
}
