using System.Collections;
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
    private bool aggro;
    private Vector2 prevPos;
    private SpriteRenderer sprite;
    private Color origColor;
    private GameObject player;

    //Doesn't do anything by design
    int IDamage.Health { get; set; }

    void Start()
    {
        // Setting default values
        player = GameObject.FindWithTag("Player");
        HP = maxHP;
        prevPos = transform.position;
        sprite = GetComponent<SpriteRenderer>();
        origColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        setBehavior();
        faceDir();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        flash();


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
        float diff = rb.transform.position.x - prevPos.x;

        // If moving right
        if(diff > 0.01f)
        {
            sprite.flipX = false;
            faceRight = true;
        
        // If moving right
        } else if(diff < -0.01f)
        {
            sprite.flipX = true;
            faceRight = false;
        }

        prevPos = transform.position;
    }

    void setBehavior()
    {
        // Figure out direction of player and distance to them
        Vector2 origin = rb.transform.position;
        Vector2 playerPos = player.transform.position;
        Vector2 dir = (playerPos - origin).normalized;
        float distance = Vector2.Distance(origin, playerPos);

        // Set ray from enemy to player location
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, dir, distance, ~enemyLayer);

        if (hit.collider.CompareTag("Player") && distance <= aggroRange) { aggro = true; }

        if (aggro) { transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime); }
        else
        {
            roam();
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

    void roam()
    {
        // Take current position
        Vector2 startPos = transform.position;
        
        if (faceRight)
        {
            // Move right until moved roam distance
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if(transform.position.x >= startPos.x + roamDist)
            {
                faceRight = false;
            }
        }

        if (!faceRight)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if(transform.position.x >= startPos.x - roamDist)
            {
                faceRight = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        aggro = false;
    }

    void flash()
    {
        StartCoroutine(flashRoutine());
    }

    IEnumerator flashRoutine()
    {
      sprite.color = Color.red;
      yield return new WaitForSeconds(0.3f);
      sprite.color = origColor;
    }
}
