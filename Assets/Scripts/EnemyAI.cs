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
    private float lastAttackTime = -999f;

    void Start()
    {
        
    }

    void Update()
    {
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
            Debug.Log("Attack: No collider in range (Check player, radius, or point");
            return;
        }
            var pc = hit.GetComponent<playerController>() ?? hit.GetComponentInParent<playerController>();

        if (pc != null)
        {
            Debug.Log("Hit Player");

            pc.takeDamage(attackDamage);
        }
        Debug.Log("Player has been hit in player layer, but no controller found");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

}
