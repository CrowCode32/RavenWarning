using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IPickup , IHeal
{
    // Movement
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] public float dashForce;
    [SerializeField] public float dashDuration;
    [SerializeField] public float dashCooldown;

    // SFX & Game Over
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private float deathFreezeDelay = 2f;
    private bool isDead = false;

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

    private float flashTimer = 0f;
    private float flashCurrentAlpha = 0f;

    // Attacks
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] float attackCooldown = 0.3f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] LayerMask enemyLayer;
    float lastAttackTime = -999f;

    // Trinket Stuff
    [SerializeField] trinket trinket;
    [SerializeField] SpriteRenderer trinketModel;


    // Feather
    [SerializeField] feather featherQueue;   // allows the player to switch feathers in the UI without affecting the game
    [SerializeField] feather feather;        // player's current feather that gives them said feather's ability

    public bool gotFeather; // check for unlocking the next feather
    private bool hasRevive = false; // Vulture
    private bool canBreakWalls = false; // Woodpecker
    private int storeJumpMax; // Roadrunner

    float horizontal = 0f;
    bool isJumping = false;
    int jumpCount;
    private float dashTimer;
    private float facingDirection = 1;
    private bool isDashing;
   
   

    void Awake()
    {
        //Time.timeScale = 1f;

        //if (!gameOverUI)
        //{
        //    var found = GameObject.FindWithTag("GameOver");
        //    if (found) gameOverUI = found;
        //}

        //if (gameOverUI)
        //{
        //    gameOverUI.SetActive(false);
        //}
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdatePlayerHPBar();
        storeJumpMax = jumpMax;

        if (damageOverlay)
        {
            var g = damageOverlay.color;
            g.a = 0f;
            damageOverlay.color = g;
        }

        if(feather !=null) 
        FeatherAbility(feather);
       
    }

    void Update()
    {
        if (isDead) return;

        featherQueue = GameManager.instance.selectedFeather;

        if(GameManager.instance.selectedTrinket != null)
        {
            trinketModel.sprite = GameManager.instance.selectedTrinket.sprite;
        }
       

       

        if (GameManager.instance.lockFeather == true)
        {

            if (feather != null) FeatherAbilityUndo(feather);
            

            Debug.Log(GameManager.instance.lockFeather);
            feather = featherQueue;

            if (feather != null) FeatherAbility(feather);
           
            
            GameManager.instance.lockFeather = false;
          
        }
        

        setAnimations();

        horizontal = 0f;

        dashTimer += Time.deltaTime;
       

        if (Input.GetKey(InputManager.instance.GetKey("Left")))
        {
            horizontal = -1f;
            facingDirection = -1;
        }

        if (Input.GetKey(InputManager.instance.GetKey("Right")))
        {
            horizontal = 1f;
            facingDirection = 1;
        }
        
        if(isDashing == false)
        {
            Movement();
        }
       
        UpdateOverlayAlpha();

        float mouseX = Input.GetAxis("Mouse X") * GameManager.instance.mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * GameManager.instance.mouseSensitivity;

        if (Input.GetKeyDown(InputManager.instance.GetKey("Fire1")))
        {
            slashAttack();
        }
    }

    void Movement()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(InputManager.instance.GetKey("Jump")) && jumpCount < jumpMax)
        {
            isJumping = true;
            jumpCount++;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }

        if (Input.GetKeyDown(InputManager.instance.GetKey("Dash")) && dashCooldown <= dashTimer)
        {
            Debug.Log("Dashing...");
            StartCoroutine(Dash());
            dashTimer = 0;
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float dashDirection = (horizontal != 0) ? horizontal : facingDirection;
        rb.linearVelocity = new Vector2( dashDirection * dashForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(dashDirection * speed, rb.linearVelocity.y);
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Vector3 normal = collision.GetContact(0).normal;
            if (normal == Vector3.up)
            {
                jumpCount = 0;
                isJumping = false;
            }
        }
    }

    public void getTrinket(trinket trinket)
    {
        Debug.Log("Adding to list...");
        // Add the trinket to the collection (UI stuff)
    }

    public void getFeather(feather feather)
    {
        Debug.Log("You got a feather!");
        gotFeather = true;
    }

    public void takeDamage(int amount)
    {
        if (amount <= 0 || isDead) return;

        UpdatePlayerHPBar();
        currentHealth = Mathf.Max(0, currentHealth - amount);
        triggerFlash();

        if (audioSource && hurtSfx)
            audioSource.PlayOneShot(hurtSfx);

        if (currentHealth <= 0)
            StartCoroutine(death());
    }

    public void UpdatePlayerHPBar()
    {
        GameManager.instance.playerHP.fillAmount = (float)currentHealth / maxHealth;
    }

    public void Heal(int amount)
    {
        if (amount > 0)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            UpdatePlayerHPBar();
        }
    }

    private IEnumerator death()
    {
        if (hasRevive)
        {
            currentHealth = (maxHealth / 2);
            GameManager.instance.hasBeenRevived = true;  
            yield break;
        }

        isDead = true;
        
        //Temp line for bug where sometimes the player dies before the UI updates
        GameManager.instance.playerHP.fillAmount = 0;

        // Stop motion and inputs
        if (rb) rb.linearVelocity = Vector2.zero;

        // Play death anim and SFX
        if (anim) anim.SetTrigger("Death");
        if (audioSource && deathSfx)
            audioSource.PlayOneShot(deathSfx);

        //Animator calls killPlayer once death animation ends
    }

    public void killPlayer()
    {
       // GameManager.instance.activeMenu = gameOverUI;
        GameManager.instance.activeMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Show Game over/lose menu and pause the game
        ///*if (gameOverUI)
        //    gameOverUI.SetActive(true);*/
        //GameManager.instance.activeMenu = gameOverUI;
        //GameManager.instance.activeMenu.SetActive(true);
        //Time.timeScale = 0f;
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        GameManager.instance.gameLost();

        Debug.Log("The Player died");
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

    // Can also be used for the enemies
    void setAnimations()
    {
        //float moveSpeed = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(horizontal));
        if (horizontal > 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (horizontal < 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }

        anim.SetBool("isJumping", isJumping);
        anim.SetInteger("jumpCount", jumpCount);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void slashAttack()
    {
        anim.SetTrigger("Slash");

        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        if (!attackPoint)
        {
            Debug.Log("Is attacking");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        if (hits.Length == 0) return;

        foreach (var h in hits)
        {
            var enemy = h.GetComponent<enemyAI>() ?? h.GetComponentInParent<enemyAI>();
            if (enemy != null)
            {
                enemy.takeDamage(attackDamage);
            }
            else if(canBreakWalls)
            {
                if(h.CompareTag("Breakable"))
                {
                    Destroy(h.gameObject);
                }
            }
        }

    }

    // This method will go in spawn/whatever the trigger is to leave the tutorial room
    void FeatherAbility(feather feather)
    {
        

       
        switch (feather.featherName)
        {
            case "Roadrunner":
                speed *= 2;
                jumpMax = 1;
                break;

            case "Woodpecker":
                canBreakWalls = true;
                break;

            case "Vulture":
                if(GameManager.instance.hasBeenRevived==false)
                hasRevive = true;
                break;

            case "Cardinal":
                Debug.Log("Tweet tweet I'm a cardinal");
                break;

            default:
                return;
        }
    }

    // Will be run right before setting feather = featherQueue
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

  
}
