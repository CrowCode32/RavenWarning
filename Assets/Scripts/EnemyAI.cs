using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.Collections;

public class enemyAI : MonoBehaviour
{

    public float speed = 1f;
    public Transform positionA;
    public Transform positionB;
    public Transform player;

    private bool movingToAttack = true;
    private float chaseDistance = 5f;
    private float attackDistance = 2f;

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
}
