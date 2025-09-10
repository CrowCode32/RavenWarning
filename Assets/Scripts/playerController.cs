using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class playerController : MonoBehaviour ,IPickup
{
    // Movment
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] LayerMask groundLayer;

    // Health
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;
    [SerializeField] private Image damageOverlay;

    // Flash and low health flash
    [SerializeField] private float flashAlpha = 0.8f;
    [SerializeField] private float flashHold = 0.15f;
    [SerializeField] private float flashFadeSpeed = 3f;
    [SerializeField] private float lowHealthThreshHold = 0.30f;
    [SerializeField] private float maxLowHealthAlpha = 0.5f;
    [SerializeField] private float minPulse = 0.75f;
    [SerializeField] private float maxPulse = 2.0f;
    [SerializeField] private float pulseResponse = 5f;

    // Trinket Stuff
    [SerializeField] trinket trinket;
    [SerializeField] GameObject trinketModel;
    public List<trinket> trinketsAquired = new List<trinket>();

    //Feather
    [SerializeField] feather featherQueue;   //allows the player to switch feathers in the UI without affecting the game
    [SerializeField] feather feather; //players current feather that gives them said feather's ability

    public bool gotFeather; //check for unlocking the next feather
    private bool hasRevive = false; //Vulture
    private bool canBreakWalls = false; //Woodpecker
    private int storeJumpMax; //Roadrunner

    float horizontal;
    int jumpCount;

    private float flashTimer = 0f;
    private float flashCurrentAlpha = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        storeJumpMax = jumpMax;
        if (!damageOverlay)
        {
            var g = damageOverlay.color;
            g.a = 0f;
            damageOverlay.color = g;
        }

        FeatherAbility(feather);
        // Whatever trinket you equip before starting will be displayed on the player after starting with this line
        // trinketModel = trinket.model;
    }
    void Update()
    {
        //featherQueue = GameManager.instance.currentFeather; 
        //When gamemanager is implemented, constantly updates to grab currently selected feather in the journal
        setAnimations();

        horizontal = Input.GetAxisRaw("Horizontal");
        Movement();
        UpdateOverlayAlpha();
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
        if (amount > 0)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
    }

    private void death()
    {
        if (hasRevive)
        {
            currentHealth = (maxHealth / 2);
        }
        else
        {
            Debug.Log("The Player died");
            

            // Add later on --- disable inputs, play death animimation, show UI maybe and respawn
        }

    }

    public void triggerFlash()
    {
        flashTimer = flashHold;
        flashCurrentAlpha = flashAlpha;
    }

    public void UpdateOverlayAlpha()
    {
        if (!damageOverlay) return;
        float damageTime = Time.deltaTime;

        if (flashTimer > 0f) flashTimer -= damageTime;

        else flashCurrentAlpha = Mathf.MoveTowards(flashCurrentAlpha, 0f, flashFadeSpeed * damageTime);

        float lowHealthAlpha = 0f;
        float healthFraction = (maxHealth > 0) ? (float)currentHealth / maxHealth : 0f;

        if (healthFraction < lowHealthThreshHold)
        {
            float i = Mathf.InverseLerp(lowHealthThreshHold, 0f, healthFraction);
            float mp = Mathf.Lerp(minPulse, maxPulse, i);
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.PI * 2f * mp);
            float targetAlpha = Mathf.Lerp(lowHealthAlpha * 0.5f, maxLowHealthAlpha, pulse);

            lowHealthAlpha = Mathf.Lerp(damageOverlay.color.a, targetAlpha, damageTime * pulseResponse);
        }

        float finalAplha = Mathf.Max(flashCurrentAlpha, lowHealthAlpha);

        var g = damageOverlay.color;
        g.a = finalAplha;
        damageOverlay.color = g;
    }

    void setAnimations()
    {
        float moveSpeed = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(moveSpeed));
        if (moveSpeed > 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
        } else if (moveSpeed < 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }
        
    }


    //This method will go in spawn/whatever the trigger is to leave the tutorial room
    void FeatherAbility(feather feather)
    {
        Debug.Log(feather.featherName);
        switch (feather.featherName)
        {
            
                case "Roadrunner":
                speed *= 2;
                jumpMax = 0;
                break;

                case "Woodpecker":
                canBreakWalls = true;
                break;

                case "Vulture":
                    hasRevive = true; 
                break;

                case "Cardinal":
                Debug.Log("Tweet tweet I'm a cardinal");
                break;

                default:
                return;
                
        }
    }

    //will be ran right before setting feather = featherQueue;
    void FeatherAbilityUndo(feather feather)
    {
        switch (feather.featherName)
        {

            case "Roadrunner":
                speed /= 2;
                jumpMax = storeJumpMax;
                break;
            case "WoodPecker":
                canBreakWalls = false;
                break;
            case "Vulture":
                hasRevive = false; 
                break;

            default:
                return;

        }
    }

    //future implementation of Woodpecker's complex ability
    void wallBreak()
    {

    }
}
