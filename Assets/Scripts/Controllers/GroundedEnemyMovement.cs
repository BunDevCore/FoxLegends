using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemyMovement : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;
    
    [Space(10)] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    
    private const float RayLength = 0.2f;
    private Animator animator;
    private bool isFacingRight = true;
    private bool shouldMove = true;

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
        var predictedMovement = rb.linearVelocity * (Time.deltaTime * 1.1f);
        var newPosition = new Vector2(transform.position.x, transform.position.y) + predictedMovement;
        if (!IsGrounded(newPosition) || IsBlockedForward(newPosition))
        {
            Flip();
        }

        rb.linearVelocityX = isFacingRight ? moveSpeed : -moveSpeed;
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private bool IsGrounded() => PhysicsRaycast(transform.position, Vector2.down);
    private bool IsGrounded(Vector3 position) => PhysicsRaycast(position, Vector2.down);
    private bool IsBlockedForward() => IsBlockedForward(transform.position);
    private bool IsBlockedForward(Vector3 position) => PhysicsRaycast(position, isFacingRight ? Vector2.right : Vector2.left);
    private bool PhysicsRaycast(Vector3 position, Vector2 direction)
    {
        //Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
        return Physics2D.BoxCast(
            position,
            new Vector2(.1f, .1F),
            0f,
            direction,
            RayLength,
            groundLayer.value
        );
    }
    
    void Death()
    {
        Debug.Log($"{gameObject.name} is kil");
        shouldMove = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!shouldMove) return;
        if (((1 << other.gameObject.layer) & groundLayer.value) != 0) return;
        Flip();
        // rb.linearVelocityX *= -1;
    }
}