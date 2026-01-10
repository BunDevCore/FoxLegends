using System;
using System.Collections.Generic;
using UnityEngine;

public class MetalHogEnemy : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;
    
    [Space(10)] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    
    private const float RayLength = 0.2f;
    private Animator animator;
    private bool isFacingRight = true;
    private bool shouldMove = true;
    
    [Header("Enemy settings")]
    [SerializeField]
    private float enemyFarSight = 0.2f; // how far enemy can see
    [SerializeField]
    private float enemyBottomSight = 0.4f; // how much below can the enemy see

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.linearVelocity = new Vector2(moveSpeed, 0);
    }

    void FixedUpdate()
    {
        if (!shouldMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (ShouldFlip()) 
            Flip();

        rb.linearVelocityX = isFacingRight ? moveSpeed : -moveSpeed;
    }

    
    private void OnDrawGizmos()
    {
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.color = Color.red;
        Gizmos.DrawRay((Vector2)transform.position + (dir * enemyFarSight), Vector2.down*enemyBottomSight);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay((Vector2)transform.position, dir * enemyFarSight);
    }
    
    private bool ShouldFlip()
    {
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 edgeOrigin = (Vector2)transform.position + (dir * enemyFarSight);
        bool isAtEdge = !Physics2D.Raycast(edgeOrigin, Vector2.down, enemyBottomSight, groundLayer);
        if (isAtEdge)
            return true;
        var inFront = Physics2D.Raycast(transform.position, dir, enemyFarSight, groundLayer);
        if (!inFront || inFront.collider.CompareTag("Player"))
            return false;
        return true;
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    
    void Death()
    {
        shouldMove = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!shouldMove) return;
        if (((1 << other.gameObject.layer) & groundLayer.value) != 0) return;
        Flip();
    }
}