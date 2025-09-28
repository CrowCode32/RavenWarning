using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Collections;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour, IDamage
{
    GameData gameData = GameManager.instance.gameData;

    private Rigidbody2D rb;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;

    // Patrol / chase
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
    [SerializeField] private float meleeAttckDuration = 0.5f;
    [SerializeField] private float deathAnimDuration = 1.0f;
    private float lastAttackTime = -999f;
    bool isAttacking = false;
    bool dead = false;

    public enum AttackType { Scream, Melee }

    // Ghost attack
    [SerializeField] int screamDamage = 1;
    [SerializeField] float windUpTime = 0.8f;
    [SerializeField] float screamDuration = 0.6f;
    [SerializeField] float damageTick = 0.5f;
    [SerializeField] float screamRange = 3f;
    [SerializeField] float screamAngle = 70f;

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

    public int Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>(true);
        if (!animator) animator = GetComponentInChildren<Animator>(true);
        if (!audioSource) audioSource = GetComponentInChildren<AudioSource>(true);
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (sprite) _origColor = sprite.color;
    }

    void Update()
    {
        if (_iFrameTimer > 0f) _iFrameTimer -= Time.deltaTime;

        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f && sprite) sprite.color = _origColor;
        }

        if (dead || !player || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Attack
        if (distanceToPlayer <= attackDistance)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetBool("Walk", false);
                if (attackType == AttackType.Melee)
                    MeleeAttack();
                else
                    StartCoroutine(ScreamAttack());
            }
            else
            {
                MoveHorizontally(player.position.x);
            }
        }
        // Chase
        else if (distanceToPlayer <= chaseDistance)
        {
            MoveHorizontally(player.position.x);
        }
        // Patrol
        else
        {
            float patrolX = (movingToAttack ? positionA.position.x : positionB.position.x);
            MoveHorizontally(patrolX);

            if (Mathf.Abs(transform.position.x - patrolX) < 0.05f)
                movingToAttack = !movingToAttack;
        }
    }

    private void MoveHorizontally(float targetX)
    {
        float dir = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        FaceDir(dir);
        animator.SetBool("Walk", true);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        if (animator) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void Attack2D()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        Vector2 g = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Collider2D hit = Physics2D.OverlapCircle(g, attackRadius, playerLayer);

        if (!hit) return;

        var pc = hit.GetComponent<playerController>() ?? hit.GetComponentInParent<playerController>();
        if (pc != null) pc.takeDamage(attackDamage);
    }

    void MeleeAttack()
    {
        lastAttackTime = Time.time;
        if (animator) { animator.ResetTrigger("Attack"); animator.SetTrigger("Attack"); }

        Vector2 origin = (Vector2)transform.position + FowardDir() * frontOffset;
        Collider2D hit = Physics2D.OverlapCircle(origin, meleeRadius, playerLayer);

        if (hit)
        {
            IDamage damageable = hit.GetComponent<IDamage>();
            if (damageable != null)
                damageable.TakeDamage(attackDamage);

            StartCoroutine(ResetAttackAnim(meleeAttckDuration));
        }
        isAttacking = false;
    }

    IEnumerator ResetAttackAnim(float duration)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    IEnumerator ScreamAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        float t = 0f;
        if (animator) animator.SetTrigger("Windup");
        while (t < windUpTime)
        {
            FaceDir(player.position.x - transform.position.x);
            t += Time.deltaTime;
            yield return null;
        }

        if (animator) { animator.ResetTrigger("Attack"); animator.SetTrigger("Attack"); }
        float elapsed = 0f;
        float nextTick = 0f;

        while (elapsed < screamDuration)
        {
            elapsed += Time.deltaTime;
            FaceDir(player.position.x - transform.position.x);

            if (elapsed >= nextTick)
            {
                nextTick += damageTick;

                Vector2 origin = transform.position;
                Collider2D[] hits = Physics2D.OverlapCircleAll(origin, screamRange, playerLayer);

                for (int i = 0; i < hits.Length; i++)
                {
                    var pc = hits[i].GetComponent<playerController>() ?? hits[i].GetComponentInParent<playerController>();
                    if (pc != null && InScreamCone(origin, hits[i].transform.position))
                        pc.takeDamage(screamDamage);
                }
            }
            yield return null;
        }
        isAttacking = false;
    }

    public void takeDamage(int amount)
    {
        if (amount <= 0 || dead) return;
        if (_iFrameTimer > 0f) return;

        _iFrameTimer = hurtFrames;
        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (sprite)
        {
            sprite.color = hitColor;
            _flashTimer = flashDuration;
        }

        if (animator) animator.SetTrigger("Hit");
        if (audioSource && hitSfx) audioSource.PlayOneShot(hitSfx);

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

    public void TakeDamage(int damageAmount)
    {
        takeDamage(damageAmount);
    }

    private void Death()
    {
        if (dead) return;
        dead = true;

        gameData.killsStat++;

        if (animator)
        {
            animator.ResetTrigger("Hit");
            animator.SetBool("Walk", false);
            animator.SetTrigger("Death");
        }
        if (audioSource && deathSfx) audioSource.PlayOneShot(deathSfx);

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.simulated = true;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        StartCoroutine(DeathGroundFreeze());
    }

    private IEnumerator DeathGroundFreeze()
    {
        var col = GetComponent<Collider2D>();
        if (col)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        float timeout = 3f;
        while (!IsGrounded() && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (col)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        float delay = deathAnimDuration;
        if (deathSfx) delay = Mathf.Max(delay, deathSfx.length);
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    private bool IsGrounded()
    {
        var col = GetComponent<Collider2D>();
        Vector2 origin;

        if (col)
            origin = new Vector2(col.bounds.center.x, col.bounds.min.y + 0.01f);
        else
            origin = (Vector2)transform.position + Vector2.down * 0.05f;

        var hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void FaceDir(float dx)
    {
        if (sprite == null) return;
        if (dx > 0.1f) sprite.flipX = false;
        else if (dx < -0.1f) sprite.flipX = true;
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

    //Not working yet
    //IEnumerator deathFade()
    //    {
    //        float alpha = sprite.color.a;
    //        Color col = sprite.color;

    //        while(sprite.color.a > 0)
    //        {
    //            Debug.Log(sprite.name);
    //            alpha -= 0.01f;
    //            col.a = alpha;
    //            sprite.color = col;

    //            yield return new WaitForSeconds(0.05f);
    //        }
    //    }
}

