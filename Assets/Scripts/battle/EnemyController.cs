using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int health = 3;
    private Transform player;
    private bool isFrozen;

    private Vector2 knockbackVelocity = Vector2.zero;
    private float knockbackTime = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isFrozen)
            return;

        if (knockbackTime > 0)
        {
            transform.Translate(knockbackVelocity * Time.deltaTime);
            knockbackTime -= Time.deltaTime;
            knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.deltaTime * 5f);
            return;
        }

        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        knockbackVelocity = force;
        knockbackTime = duration;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GlowingSphere"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
       
        else if (other.CompareTag("Player"))
        {
            PlayerBattle playerHealth = other.GetComponent<PlayerBattle>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }

        Light2D light = GetComponent<Light2D>();
        if (light != null)
        {
            light.color = Color.cyan; 
            StartCoroutine(ChangeColorAfterDelay(light, 1f));
        }
    }

    private IEnumerator ChangeColorAfterDelay(Light2D light, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (light != null)
        {
            light.color = Color.white; 
        }
    }

    public void FreezeMovement(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

}
