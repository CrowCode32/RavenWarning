using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Collections;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour
{
    // Patrol
    public float speed = 3f;
    public Transform positionA;
    public Transform positionB;
    public Transform player;
    bool movingToAttack = true;
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
    bool isAttacking = false;

    public enum AttackType { Scream, Melee }

    // Ghost attack
    [SerializeField] int screamDamage = 1;
    [SerializeField] float windUpTime = 0.8f;
    [SerializeField] float screamDuration = 0.6f;
    [SerializeField] float damageTick = 0.5f;
    [SerializeField] float screamRange = 3f;
    [SerializeField] float screamAngle = 70f;

    // Knight attack


    // Slashing attack
    [SerializeField] AttackType attackType = AttackType.Melee;
    [SerializeField] float frontOffset = 0.5f;
    [SerializeField] float meleeRadius = 0.5f;

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
        // Timers
        if (_iFrameTimer > 0f) _iFrameTimer -= Time.deltaTime; // Invulnerability

        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f && sprite) sprite.color = _origColor;
        }

        if (!player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Attack   
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackDistance)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    if (attackType == AttackType.Melee)
                    {
                        MeleeAttack();
                    }
                    else if (attackType == AttackType.Scream)
                    {
                        StartCoroutine(ScreamAttack());
                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);  
                }
            }
            // Chase
            else if (distanceToPlayer <= chaseDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            // Patrol
            else
            {
                Vector2 target = (movingToAttack ? positionA : positionB).position;
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                FaceDir(target.x - transform.position.x);

                if (Vector2.Distance(transform.position, target) < 0.1f) movingToAttack = !movingToAttack;
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

  // Melee slashing attack
    void MeleeAttack()
    {
        lastAttackTime = Time.time;
        if (animator)
            animator.SetTrigger("Attack");

        Vector2 origin = (Vector2)transform.position + FowardDir() * frontOffset;
        Collider2D hit = Physics2D.OverlapCircle(origin, meleeRadius, playerLayer);

        // --- This is the corrected part ---
        if (hit)
        {
            // Instead of looking for a specific controller, we look for any
            // component that can be damaged.
            IDamage damageable = hit.GetComponent<IDamage>();
            if (damageable != null)
            {
                // Now it can correctly find our PlayerHealthBridge.
                Debug.Log("Enemy hit the player!");
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    // Screaming or howl attack
    IEnumerator ScreamAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // WindUp
        float t = 0f;
        if (animator)
            animator.SetTrigger("Windup");
        while (t < windUpTime)
        {
            FaceDir(player.position.x - transform.position.x);
            t += Time.deltaTime;
            yield return null;
        }

        // Scream tick in cone shape
        if (animator)  
            animator.SetTrigger("Attack");
        float elasped = 0f;
        float nextTick = 0f;

        while (elasped < screamDuration)
        {
            elasped += Time.deltaTime;
            FaceDir(player.position.x - transform.position.x);

            if (elasped >= nextTick)
            {
                nextTick += damageTick;

                // Get colliders in cone range
                Vector2 origin = transform.position;
                Collider2D[] hits = Physics2D.OverlapCircleAll(origin, screamRange, playerLayer);

                for (int i = 0; i < hits.Length; i++)
                {
                    var pc = hits[i].GetComponent<playerController>() ?? hits[i].GetComponentInParent<playerController>();

                    if (pc != null && InScreamCone(origin, hits[i].transform.position))
                    {
                        pc.takeDamage(screamDamage);
                    }
                }
            }
            yield return null;
        }
        isAttacking = false;
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
        if (audioSource && hitSfx) 
            audioSource.PlayOneShot(hitSfx);

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
        if (audioSource && deathSfx)
            audioSource.PlayOneShot(deathSfx); 
        
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        enabled = false;

        Destroy(gameObject, deathSfx ? deathSfx.length : 0f);
        // Todo - play death anim/SFX, add score or drop loot
    }

    private void FaceDir(float dx)
    {
        if (sprite == null) return;
        if (dx > 0.1f)
            sprite.flipX = false; // Right
        else if (dx < -0.1f)
            sprite.flipX = true; // Left
    }

    Vector2 FowardDir()
    {
        if (!sprite) return Vector2.right;
        return sprite.flipX ? Vector2.left : Vector2.right;
    }

    bool InScreamCone(Vector2 origin, Vector2 targetPos)
    {
        Vector2 toTarget = targetPos - origin;
        float dist = toTarget.magnitude;
        if (dist > screamRange || dist < Mathf.Epsilon) return false;

        Vector2 fwd = FowardDir().normalized;
        float cos = Vector2.Dot(fwd, toTarget.normalized);
        float limit = Mathf.Cos(screamAngle * Mathf.Deg2Rad);
        return cos >= limit;
    }
}
