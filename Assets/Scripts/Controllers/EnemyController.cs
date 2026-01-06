using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Space(10)] private Rigidbody2D rb;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Debug.Log("enemycontroller, innit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator KillOnAnimationEnd()
    {
        rb.simulated = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (transform.position.y < other.gameObject.transform.position.y)
            {
                gameObject.SendMessage("Death");
                other.gameObject.SendMessage("KilledEnemy");
                //animator.SetBool("isDead", true);
                StartCoroutine(KillOnAnimationEnd());
            }
            else
            {
                other.gameObject.SendMessage("PlayerDeath");
            }
        }
    }
}
