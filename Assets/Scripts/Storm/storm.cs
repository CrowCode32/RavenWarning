using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class storm : MonoBehaviour
{
    [SerializeField] int speed;

    Vector3 startPos;
    Vector3 endPos;   // last possible end point
    float maxDistance;

    // A very high damage value to ensure an instant kill.
    private const int LETHAL_DAMAGE = 9999;

    private void Start()
    {
        startPos = GameManager.instance.stormSpawnPoint.transform.position;
        endPos = GameManager.instance.stormEndPoint.transform.position;
        maxDistance = Vector3.Distance(startPos, endPos);
    }

    // Update is called once per frame
    void Update()
    {
        //Move the storm 
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //Get the current distance to the end and fill the bar accordingly
        float stormDistance = Vector3.Distance(transform.position, endPos);
        GameManager.instance.stormFill.fillAmount = Mathf.InverseLerp(maxDistance, 0, stormDistance);
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

    private void OnDestroy()
    {
        GameManager.instance.stormOffset = Vector2.Distance(transform.position, endPos);
        GameManager.instance.stormFill.fillAmount = 0;
    }
}
