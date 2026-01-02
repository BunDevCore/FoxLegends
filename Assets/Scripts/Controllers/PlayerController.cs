using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField] private float jumpForce = 6.0f;
    [Space(10)] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayer;
    private const float rayLength = 0.2f;
    private Animator animator;
    private bool isRunning = false;
    private bool isFacingRight = true;
    private Vector2 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
        
        isRunning = moveInput != 0;
        
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        rigidBody.linearVelocity = new Vector2(moveInput * moveSpeed, rigidBody.linearVelocity.y);

        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", rigidBody.linearVelocity.y < -0.1f);
    }

    bool IsGrounded()
    {
        //Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
        return Physics2D.BoxCast(
            this.transform.position,
            new Vector2(.1f, .1F),
            0f,
            Vector2.down,
            rayLength,
            groundLayer.value
        );
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("MovingPlatform"))
        {
            transform.SetParent(col.transform);
        }

        // if (col.CompareTag("LevelExit"))
        // {
        //     if (GameManager.instance.keysCompleted)
        //     {
        //         GameManager.instance.LevelCompleted();
        //         Debug.Log("lewel complete");
        //     }
        //     else
        //     {
        //         Debug.Log("idź tam klucze zbierać a nie się wygłupiasz");
        //     }
        // }

        if (col.CompareTag("Killzone"))
        {
            GameManager.instance.AddLives(-1);
            transform.position = GameManager.instance.currentSpawnPoint;
            rigidBody.linearVelocity = Vector2.zero;
        }
    }
}