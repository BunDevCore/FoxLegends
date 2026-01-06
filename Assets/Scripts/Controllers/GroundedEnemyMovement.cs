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
    private bool isRunning = false;
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private HashSet<GameObject> ignoredCollisions = new();

    void Awake()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.linearVelocity = new Vector2(moveSpeed, 0);
    }

    void FixedUpdate()
    {
        var predictedMovement = rb.linearVelocity * (Time.deltaTime * 1.1f);
        var newPosition = new Vector2(transform.position.x, transform.position.y) + predictedMovement;
        if (IsBlockedForward(newPosition))
        {
            Debug.Log("forward gówno");
        }
        if (!IsGrounded(newPosition))
        {
            Debug.Log("nie grounded gówno");
        }
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer.value) != 0) return;
        Flip();
        // rb.linearVelocityX *= -1;
        ignoredCollisions.Add(other.gameObject);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        ignoredCollisions.Remove(other.gameObject);
    }
}