using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class storm : MonoBehaviour
{
    [SerializeField] int speed;

    Vector3 endPos;   // last possible end point
    float exitDiff = 0;

    // A very high damage value to ensure an instant kill.
    private const int LETHAL_DAMAGE = 9999;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //Determining how far the storm is from the exit for offset in future levels
        exitDiff = endPos.x - transform.position.x;
        GameManager.instance.stormOffset = exitDiff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit has a component that can be damaged.
        IDamage damageable = collision.GetComponent<IDamage>();

        // If it's a damageable object (like the player, an NPC, or an enemy)...
        if (damageable != null)
        {
            // ...deal lethal damage to it.
            Debug.Log("Storm has hit " + collision.name + ". Dealing lethal damage.");
            damageable.TakeDamage(LETHAL_DAMAGE);
        }
    }
}
