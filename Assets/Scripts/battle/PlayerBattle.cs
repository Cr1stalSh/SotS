using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerBattle : MonoBehaviour
{
    public int health = 10;
    public float moveSpeed = 5f;

    public GameObject spherep; 
    public float sphereSpeed = 7f;  
    public int numBalls = 10; 
    public float radius = 10f; 

    // Параметры для навыков
    public float repelForce = 500f;       
    public float ghostDuration = 3f;      
    public float freezeDuration = 2f;     

    // Таймеры для перезарядки способностей
    public float repelCooldown = 5f;   
    public float ghostCooldown = 10f;  
    public float freezeCooldown = 7f;  
    public float shootCooldown = 0.5f; 
    public float dropCooldown = 3f;    

    private float lastRepelTime = -100f;
    private float lastGhostTime = -100f;
    private float lastFreezeTime = -100f;
    private float lastShootTime = -100f;
    private float lastDropTime = -100f;

    private SpriteRenderer spriteRenderer;
    private Collider2D[] playerColliders;

    public static event Action<int, int> OnHealthChanged;
    public static int CurrentHealth { get; private set; }
    public static int MaxHealth { get; private set; }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerColliders = GetComponents<Collider2D>();
        MaxHealth = health;
        CurrentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (Time.time >= lastShootTime + shootCooldown)
            ShootGlowingBall();

        if (GameManager.Instance.IsAbilityUnlocked(Ability.Freeze)
            && Time.time >= lastFreezeTime + freezeCooldown
            && Input.GetKeyDown(KeyCode.F))
        {
            FreezeEnemies();
            GameManager.Instance.NotifyAbilityUsed(Ability.Freeze);
            lastFreezeTime = Time.time;
        }

        if (GameManager.Instance.IsAbilityUnlocked(Ability.Repel)
            && Time.time >= lastRepelTime + repelCooldown
            && Input.GetKeyDown(KeyCode.Q))
        {
            RepelEnemies();
            GameManager.Instance.NotifyAbilityUsed(Ability.Repel);
            lastRepelTime = Time.time;
        }

        if (GameManager.Instance.IsAbilityUnlocked(Ability.Ghost)
            && Time.time >= lastGhostTime + ghostCooldown
            && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ActivateGhostMode());
            GameManager.Instance.NotifyAbilityUsed(Ability.Ghost);
            lastGhostTime = Time.time;
        }

        if (Time.time >= lastDropTime + dropCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            DropGlowingBalls();
            lastDropTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        CurrentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        Debug.Log("Игрок получил урон. Осталось здоровья: " + health);
        if (health <= 0)
        {
            Die();
        }
        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.intensity = 10;
            light.pointLightOuterRadius = 16;
            StartCoroutine(ChangeColorAfterDelay(light, 0.5f));
        }
    }

    private IEnumerator ChangeColorAfterDelay(Light2D light, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (light != null)
        {
            light.intensity = 1;
            light.pointLightOuterRadius = 8;
        }
    }

    void Die()
    {
        Debug.Log("Игрок погиб!");
        GameManager.Instance.NotifyLevelRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void DropGlowingBalls()
    {
        for (int i = 0; i < numBalls; i++)
        {
            float angle = i * (360f / numBalls);
            float radians = angle * Mathf.Deg2Rad;
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * radius;
            Instantiate(spherep, spawnPosition, Quaternion.identity);
        }
    }

    void ShootGlowingBall()
    {
        Vector2 shootDirection = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            shootDirection = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            shootDirection = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            shootDirection = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            shootDirection = Vector2.right;

        if (shootDirection != Vector2.zero)
        {
            GameObject sphere = Instantiate(spherep, transform.position, Quaternion.identity);
            Rigidbody2D rb = sphere.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootDirection * sphereSpeed;
            }

            lastShootTime = Time.time; 
        }
    }

    void RepelEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemyController.ApplyKnockback(direction * repelForce, 0.2f);
            }
        }
    }

    IEnumerator ActivateGhostMode()
    {
        foreach (Collider2D col in playerColliders)
        {
            col.enabled = false;
        }

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            Color transparentColor = originalColor;
            transparentColor.a = 0.3f;
            spriteRenderer.color = transparentColor;
        }

        yield return new WaitForSeconds(ghostDuration);

        foreach (Collider2D col in playerColliders)
        {
            col.enabled = true;
        }
        if (spriteRenderer != null)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = 1f;
            spriteRenderer.color = currentColor;
        }
    }

    void FreezeEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.FreezeMovement(freezeDuration);
            }
        }
    }
}
