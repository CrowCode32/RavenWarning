using UnityEngine;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] LayerMask groundLayer;

    float horizontal;
    int jumpCount;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Movement();
    }

    void Movement()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Ground") && rb.transform.position.y > (collision.collider.transform.position.y + 1))
        {
            Debug.Log("JumpsReset");
            jumpCount = 0;
        }
    }
}
