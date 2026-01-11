using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField] private float jumpForce = 6.0f;
    [Space(10)] public Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayer;
    private const float rayLength = 0.2f;
    public Animator animator;
    private bool isRunning = false;
    private bool isFacingRight = true;
    private Vector2 startPosition;

    public bool isDead = false;
    public bool enableMovement = true;

    public bool isOnMovingPlatform = false;
    private Rigidbody2D movingPlatformRb;

    private GameObject oneWayPlatform;
    private BoxCollider2D playerCollider;

    [Header("Grappling hook settings")] public bool isGrappling = false;
    [SerializeField] private LayerMask grappleLayer;

    [SerializeField] private float grappleMaxLength = 1f;
    [SerializeField] private float dampingForce = 0.1f;
    private float grappleLength;
    private Vector3 grapplePoint;
    private DistanceJoint2D distanceJoint;
    private LineRenderer rope;

    void Awake()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();

        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;
        rope = GetComponent<LineRenderer>();
        rope.enabled = false;
    }

    void Update()
    {
        float moveInput = 0f;
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && enableMovement) moveInput = 1f;
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && enableMovement) moveInput = -1f;
        if (isGrappling)
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && enableMovement)
                if (grappleLength < grappleMaxLength * 1.25f)
                    grappleLength += 0.01f;
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && enableMovement)
                if (grappleLength > 0.01f)
                    grappleLength -= 0.01f;
        }
        else
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && enableMovement && oneWayPlatform)
                StartCoroutine(DisableCollision());
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && IsGrounded() && enableMovement)
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetMouseButtonDown(0) && enableMovement)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero,
                Mathf.Infinity,
                grappleLayer
            );
            if (hit.collider)
            {
                float dist = Vector2.Distance(transform.position, hit.point);
                if (dist > grappleMaxLength) return;

                rigidBody.linearDamping = dampingForce;

                grapplePoint = hit.point;
                grapplePoint.z = 0;

                distanceJoint.connectedAnchor = grapplePoint;
                distanceJoint.distance = grappleLength = dist;
                distanceJoint.enabled = true;

                rope.SetPosition(0, grapplePoint);
                rope.SetPosition(1, transform.position);
                rope.enabled = true;

                isGrappling = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && enableMovement)
        {
            rigidBody.linearDamping = 0;
            isGrappling = false;
            distanceJoint.enabled = false;
            rope.enabled = false;
        }

        if (rope.enabled)
        {
            distanceJoint.distance = grappleLength;
            rope.SetPosition(1, transform.position);
        }

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();

        isRunning = moveInput != 0;


        if (isGrappling)
            rigidBody.linearVelocity =
                new Vector2(rigidBody.linearVelocity.x + moveInput * 0.005f, rigidBody.linearVelocity.y);
        else
            rigidBody.linearVelocity =
                new Vector2(moveInput * moveSpeed + (isOnMovingPlatform ? movingPlatformRb.linearVelocity.x : 0),
                    rigidBody.linearVelocity.y);


        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", rigidBody.linearVelocity.y < -0.1f && !isOnMovingPlatform);
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
            isOnMovingPlatform = false;
            movingPlatformRb = null;
            rigidBody.gravityScale = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("MovingPlatform"))
        {
            isOnMovingPlatform = true;
            movingPlatformRb = col.GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 1.5f;
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
            PlayerDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            oneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            oneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        TilemapCollider2D platformCollider = oneWayPlatform.GetComponent<TilemapCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    private void PlayerDeath()
    {
        if (isDead) return;
        animator.SetBool("isDead", true);
        GameManager.instance.AddLives(-1);
        StartCoroutine(GameManager.instance.RespawnSequence(this));
    }
}