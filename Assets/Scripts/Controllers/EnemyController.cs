using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Space(10)] private Rigidbody2D rb;
    [SerializeField] private float maxBounceVelocity = 3.0f;
    [SerializeField] private float enemyBounceVelocity = 2.0f;
    [Header("Explosion Prefab")]
    [SerializeField] private GameObject explosionPrefab;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    IEnumerator KillOnAnimationEnd()
    {
        animator.enabled = false;
        rb.simulated = false;
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 hitNormal = other.GetContact(0).normal;
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            // Gracz zabija przeciwnika tylko jeśli:
            // - hitNormal.y < -0.5f (uderzenie przyszło z góry)
            // - playerRb.linearVelocity.y < 0 (gracz faktycznie spada/skacze na świnię)

            if (hitNormal.y < -0.5f && playerRb.linearVelocity.y < 0.1f)
            {
                gameObject.SendMessage("Death");
                playerRb.linearVelocityY += enemyBounceVelocity;
                if (playerRb.linearVelocityY > maxBounceVelocity) playerRb.linearVelocityY = maxBounceVelocity;
                StartCoroutine(KillOnAnimationEnd());
            }
            else
                other.gameObject.SendMessage("PlayerDeath");
        }
        
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Killzone"))
        {
            StartCoroutine(KillOnAnimationEnd());
        }
    }
}