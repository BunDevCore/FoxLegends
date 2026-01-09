using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BunnyController : MonoBehaviour
{
    [Header("Bunny settings")] [SerializeField]
    private float moveSpeed = 1.5f;
    [SerializeField] private float moveRange = 1.0f;
    [SerializeField] private float minWaitTime = 1.0f;
    [SerializeField] private float maxWaitTime = 4.0f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isMoving = false;
    private bool isFacingRight = true;
    private Animator animator;

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(startPosition, Vector2.left * moveRange);
    //     Gizmos.DrawRay(startPosition, Vector2.right * moveRange);
    // }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Collider2D bunnyCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(bunnyCollider, playerCollider);
        }

        StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            if (animator) animator.SetBool("isRunning", false);
            yield return new WaitForSeconds(waitTime);
            float targetX = Random.Range(startPosition.x - moveRange, startPosition.x + moveRange);
            yield return StartCoroutine(MoveToX(targetX));
        }
    }

    IEnumerator MoveToX(float targetX)
    {
        isMoving = true;
        if (animator) animator.SetBool("isRunning", true);
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            float direction = targetX > transform.position.x ? 1 : -1;
            if (direction > 0 && !isFacingRight) Flip();
            else if (direction < 0 && isFacingRight) Flip();
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            yield return null;
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isMoving = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}