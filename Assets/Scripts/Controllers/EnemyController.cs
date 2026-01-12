using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D box;
    [SerializeField] private float maxBounceVelocity = 3.0f;
    [SerializeField] private float enemyBounceVelocity = 2.0f;
    [Header("Explosion Prefab")]
    [SerializeField] private GameObject explosionPrefab;
    private Animator animator;
    [SerializeField] private CameraFollow mCameraFollow;
    
    [Header("Audio Settings")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        mCameraFollow = Camera.main?.GetComponent<CameraFollow>();
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator KillOnAnimationEnd()
    {
        animator.enabled = false;
        box.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.linearVelocity = Vector2.zero;
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
                mCameraFollow.Shake(.3f);
                GameManager.instance.AddPoints(5);
                playerRb.linearVelocityY += enemyBounceVelocity;
                if (playerRb.linearVelocityY > maxBounceVelocity) playerRb.linearVelocityY = maxBounceVelocity;
                if (audioSource != null && deathSound != null)
                {
                    audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(deathSound, AudioListener.volume);
                    audioSource.pitch = 1f;
                }
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