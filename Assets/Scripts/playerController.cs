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

    // SFX & Game Over
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private float deathFreezeDelay = 0.75f;
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
    [SerializeField] GameObject trinketModel;

    // Feather
    [SerializeField] feather featherQueue;   // allows the player to switch feathers in the UI without affecting the game
    [SerializeField] feather feather;        // player's current feather that gives them said feather's ability

    public bool gotFeather; // check for unlocking the next feather
    private bool hasRevive = false; // Vulture
    private bool canBreakWalls = false; // Woodpecker
    private int storeJumpMax; // Roadrunner

    float horizontal;
    bool isJumping = false;
    int jumpCount;

    void Awake()
    {
        Time.timeScale = 1f;

        if (!gameOverUI)
        {
            var found = GameObject.FindWithTag("GameOver");
            if (found) gameOverUI = found;
        }

        if (gameOverUI)
        {
            gameOverUI.SetActive(false);
        }
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

        FeatherAbility(feather);
        // trinketModel = trinket.model; // future equip visuals
    }

    void Update()
    {
        if (isDead) return;

        featherQueue = GameManager.instance.selectedFeather;
        setAnimations();

        horizontal = Input.GetAxisRaw("Horizontal");
        Movement();
        UpdateOverlayAlpha();

        if (Input.GetButtonDown("Fire1"))
        {
            slashAttack();
        }
    }

    void Movement()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            isJumping = true;
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

        if (collision.collider.CompareTag("Ground"))
        {
            isJumping = false;
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
            yield break;
        }

        isDead = true;

        // Stop motion and inputs
        if (rb) rb.linearVelocity = Vector2.zero;

        // Play death anim and SFX
        if (anim) anim.SetTrigger("Death");
        if (audioSource && deathSfx)
            audioSource.PlayOneShot(deathSfx);

        // Delay to see the player fall over
        yield return new WaitForSeconds(deathFreezeDelay);

        // Show Game over/lose menu and pause the game
        if (gameOverUI)
            gameOverUI.SetActive(true);
        Time.timeScale = 0f;

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
        float moveSpeed = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(moveSpeed));
        if (moveSpeed > 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveSpeed < 0)
        {
            rb.GetComponent<SpriteRenderer>().flipX = true;
        }

        anim.SetBool("isJumping", isJumping);
        anim.SetInteger("jumpCount", jumpCount);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void slashAttack()
    {
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
        }

        if (anim) anim.SetTrigger("Slash");
    }

    // This method will go in spawn/whatever the trigger is to leave the tutorial room
    void FeatherAbility(feather feather)
    {
        if (feather == null) return;

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

    // future implementation of Woodpecker's complex ability
    void wallBreak()
    {

    }
}
