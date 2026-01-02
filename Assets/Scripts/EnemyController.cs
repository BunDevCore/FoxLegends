using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;
    private bool isFacingRight = false;
    private Animator animator;
    private float startPositionX;
    [SerializeField]
    private float moveRange = 1.0f;

    private bool isMovingRight = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        startPositionX = transform.position.x;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {
            if (this.transform.position.x < startPositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                isMovingRight = false;
                Flip();
            }
        }
        else
        {
            if (this.transform.position.x > startPositionX)
            {
                MoveLeft();
            }
            else
            {
                isMovingRight = true;
                Flip();
            }
        }
    }

    IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (transform.position.y < col.gameObject.transform.position.y)
            {
                animator.SetBool("isDead", true);
                StartCoroutine(KillOnAnimationEnd());
            }
        }
    } 

    void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
    
    void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
