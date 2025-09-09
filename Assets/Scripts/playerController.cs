using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class playerController : MonoBehaviour ,IPickup
{
    // Player
    [SerializeField] Rigidbody2D rb;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;
    

    // Trinket Stuff
    [SerializeField] trinket trinket;
    [SerializeField] GameObject trinketModel;
    public List<trinket> trinketsAquired = new List<trinket>();

    public bool gotFeather;
    float horizontal;
    int jumpCount;

    void Start()
    {
        currentHealth = maxHealth;

    



        // Whatever trinket you equip before starting will be displayed on the player after starting with this line
        // trinketModel = trinket.model;
    }
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
            jumpCount = 0;
        }
    }

    public void getTrinket(trinket trinket)
    {
        Debug.Log("Adding to list...");
        trinketsAquired.Add(trinket);

        //Add the trinket to the collection(UI stuff)
    }

    public void getFeather(feather feather)
    {
        Debug.Log("You got a feather!");
        gotFeather = true;
    }

    public void takeDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        triggerFlash();

        if (currentHealth <= 0)
            death();
    }

    public void heal(int amount)
    {
        if (amount <= 0)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
    }

    private void death()
    {
        Debug.Log("The Player died");

        // Add later on --- disable inputs, play death animimation, show UI maybe and respawn
    }

    public void triggerFlash()
    { 
        
    }
}
