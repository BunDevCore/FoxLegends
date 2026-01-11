using System.Collections;
using UnityEngine;

public class MainMenuJumper : MonoBehaviour
{
    [Header("Movement parameters")] [SerializeField]
    private float jumpForce = 6.0f;

    [SerializeField] private LayerMask groundLayer;
    private const float rayLength = 0.2f;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private bool queuedJump = false;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (IsGrounded() && rigidBody.linearVelocityY <= 0) StartCoroutine(DelayJump());
        
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isFalling", rigidBody.linearVelocity.y < -0.1f);
    }

    IEnumerator DelayJump()
    {
        if (!queuedJump)
        {
            queuedJump = true;
            yield return new WaitForSeconds(1f);
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            queuedJump = false;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.BoxCast(
            this.transform.position,
            new Vector2(.1f, .1F),
            0f,
            Vector2.down,
            rayLength,
            groundLayer.value
        );
    }
}