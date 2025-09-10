using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Collections;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour
{
    // Patrol
    public float speed = 1f;
    public Transform positionA;
    public Transform positionB;
    public Transform player;
    private bool movingToAttack = true;
    private float chaseDistance = 5f;
    private float attackDistance = 2f;

    // Attacks
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;
    private float lastAttackTime = -999f;

    // Hit feedback
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float hurtFrames = 0.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private float knockback = 4f;


    private Color _origColor;
    private float _flashTimer = 0f;
    private float _iFrameTimer = 0f;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (sprite) _origColor = sprite.color;
    }

    void Update()
    {
        if (_iFrameTimer > 0f) _iFrameTimer -= Time.deltaTime; // Invulnerability

        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f && sprite) sprite.color = _origColor;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Attack distance
        if (distanceToPlayer <= attackDistance)
        {
            Debug.Log("Attacking the player");
            Attack2D();
        }

        // Player chase
        else if (distanceToPlayer <= chaseDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

        // Switch between the two points --- So patrolling
        else
        {
            Vector2 targetDistance = movingToAttack ? positionA.position : positionB.position;
            transform.position = Vector2.MoveTowards(transform.position, targetDistance, speed * Time.deltaTime);

            // Switch the direction
            if (Vector2.Distance(transform.position, targetDistance) < 0.1f)
            {
                movingToAttack = !movingToAttack;
            }
        }
    }

    public void Attack2D()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        Vector2 g = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D hit = Physics2D.OverlapCircle(g, attackRadius, playerLayer);

        if (!hit)
        {
            return;
        }
            var pc = hit.GetComponent<playerController>() ?? hit.GetComponentInParent<playerController>();

        if (pc != null)
        {
            pc.takeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    public void takeDamage(int amount)
    {
        if (amount <= 0) return;

        if (_iFrameTimer > 0f) return;

        _iFrameTimer = hurtFrames;
        currentHealth = Mathf.Max(0, currentHealth - amount);

        // Hit anim
        if (animator)
            animator.SetTrigger("Hit");
        if (sprite)
        {
            sprite.color = hitColor;
            _flashTimer = flashDuration;
        }

        // Sound SFX
        if (audioSource && hitSfx) audioSource.PlayOneShot(hitSfx);

        // Knoackback --- VOID this out if we don't need it
        if (player)
        {
            var rb = GetComponent<Rigidbody2D>();
            if (rb)
            {
                Vector2 dir = (transform.position - player.position).normalized;
                rb.AddForce(dir * knockback, ForceMode2D.Impulse);
            }    
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }
    private void Death()
    {
        Destroy(gameObject);
        // Todo - play death anim/SFX, add score or drop loot
    }
}
