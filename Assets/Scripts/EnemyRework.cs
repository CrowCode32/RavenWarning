using UnityEngine;
using UnityEngine.UIElements;

public class EnemyRework : MonoBehaviour, IDamage
{
    [Header("Stats")]
    [SerializeField] int maxHP;
    [SerializeField] int HP;
    [SerializeField] int speed;
    [Tooltip("The distance from the current position the enemy may roam.")]
    [SerializeField] float roamDist;
    [SerializeField] float aggroRange;
    [Header("References")]
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;
    //Place holder for if we implement audio feedback for enemies later
    /* [SerializeField] AudioSource audio;
    [SerializeField] AudioClip hitSfx;
    [SerializeField] AudioClip deathSfx;*/


    private bool dead;
    private bool faceRight;
    private GameObject player;

    //Doesn't do anything by design
    int IDamage.Health { get; set; }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        faceDir();
        setRoam();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if(HP <= 0)
        {

        }

    }

    void death()
    {
        dead = true;
        GameManager.instance.gameData.killsStat++;

        if(anim != null)
        {
            anim.SetTrigger("Death");
        }
    }

    //Called by animator at the end of death animation
    void destroy()
    {
        Destroy(gameObject);
    }

    void faceDir()
    {
        // If moving right
        if(rb.linearVelocityX >= 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
            faceRight = true;
        
        // If moving right
        } else if(rb.linearVelocityX < 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
            faceRight = false;
        }
    }

    void setRoam()
    {
        // Figure out direction of player and distance to them
        Vector2 origin = rb.transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 dir = (playerPos - origin).normalized;
        float distance = Vector2.Distance(origin, playerPos);

        // Set ray from enemy to player location
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, dir, distance, ~enemyLayer);

        if (hit.collider.CompareTag("Player") && distance <= aggroRange)
        {
            
        }
        
        
        
        // Ray for debug
        Color rayColor = Color.white;
        if(hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player")) { rayColor = Color.green; }

        }
        Debug.DrawRay(rb.transform.position, dir * distance, rayColor);
    }


}
