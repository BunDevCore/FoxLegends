using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QueuedMovementInput
{
    public bool jump;
    public bool hold;
    public float directionalInput;
    public bool phaseDown;
    public bool grappleExtend;
    public bool grappleContract;
    public bool grappleDown;
    public bool grappleUp;

    public void Reset()
    {
        jump = false;
        hold = false;
        directionalInput = 0f;
        phaseDown = false;
        grappleExtend = false;
        grappleContract = false;
        grappleUp = false;
        grappleDown = false;
    }
}

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
    public int maxCoyoteFrames = 3;
    private int coyoteFrames = 0;
    public int maxJumpHoldFrames = 20;
    public int jumpHoldFrames = 0;
    public float jumpHoldFalloffExp = 0.99f;
    public float rozbieg = 0.2f;
    private bool jumpUsed = false;
    private bool jumpHeld = false;
    private float currentJumpForce;
    private bool isGrounded = true;
    private bool wasGroundedLastFrame = true;

    public bool isOnMovingPlatform = false;
    private GameObject movingPlatform;

    private GameObject oneWayPlatform;
    private BoxCollider2D playerCollider;

    [Header("Grappling hook settings")] public bool isGrappling = false;
    [SerializeField] private LayerMask grappleLayer;

    [SerializeField] private float grappleGrabMaxLength = 1f;
    [SerializeField] private float grappleMaxLength = 1.5f;
    [SerializeField] private float swingAcceleration = 0.01f;
    [SerializeField] private float dampingForce = 0.1f;
    private float grappleLength;
    private Vector3 grapplePoint;
    private DistanceJoint2D distanceJoint;
    private LineRenderer rope;

    [Header("Audio Settings")] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip bonusSound;

    private GameObject originalParent;
    private QueuedMovementInput queuedMovement = new();

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
        audioSource = GetComponent<AudioSource>();
        isGrappling = false;

        originalParent = gameObject.transform.parent.gameObject;
    }

    private void FixedUpdate()
    {
        HandleJump();
        DoGrapplePhysics();

        if (!isGrappling)
        {
            if (queuedMovement.phaseDown)
                StartCoroutine(DisableCollision());
        }

        if (isGrappling)
        {
            rigidBody.linearVelocity =
                new Vector2(rigidBody.linearVelocity.x + queuedMovement.directionalInput * .05f, rigidBody.linearVelocity.y);
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(queuedMovement.directionalInput * moveSpeed, rigidBody.linearVelocity.y);
        }


        queuedMovement.Reset();
    }

    void Update()
    {
        CycleGroundedState();
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && enableMovement) queuedMovement.directionalInput = 1f;
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && enableMovement) queuedMovement.directionalInput = -1f;

        if (Input.GetMouseButtonUp(0))
        {
            queuedMovement.grappleDown = false;
            queuedMovement.grappleUp = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            queuedMovement.grappleDown = true;
            queuedMovement.grappleUp = false;
        }

        if (!isGrappling)
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && enableMovement && oneWayPlatform)
                queuedMovement.phaseDown = true;
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && enableMovement)
                queuedMovement.jump = true;
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && enableMovement)
                queuedMovement.hold = true;
        }
        else
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && enableMovement)
            {
                queuedMovement.grappleExtend = true;
            }

            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && enableMovement)
            {
                queuedMovement.grappleContract = true;
            }
        }


        if (queuedMovement.directionalInput > 0 && !isFacingRight) Flip();
        else if (queuedMovement.directionalInput < 0 && isFacingRight) Flip();

        isRunning = queuedMovement.directionalInput != 0;

        if (rope.enabled)
        {
            distanceJoint.distance = grappleLength;
            rope.SetPosition(1, transform.position);
        }

        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", rigidBody.linearVelocity.y < -0.1f && !isOnMovingPlatform);
    }

    private void HandleJump()
    {
        bool wantsToHold = queuedMovement.hold;
        bool wantsToJump = queuedMovement.jump;
        if (isGrounded)
        {
            if (rigidBody.linearVelocity.y <= 0.1f)
            {
                coyoteFrames = maxCoyoteFrames;
                jumpUsed = false;
                jumpHeld = false;
                jumpHoldFrames = 0;
            }
        }
        else
        {
            coyoteFrames--;
        }

        if (jumpUsed && jumpHeld && jumpHoldFrames < maxJumpHoldFrames)
        {
            jumpHoldFrames++;
            Debug.Log("jump hold frames: " + jumpHoldFrames);
            HoldJump();
        }

        if (!wantsToHold)
        {
            jumpHeld = false;
            return;
        }

        if (!wantsToJump || jumpUsed || coyoteFrames <= 0)
        {
            return;
        }

        jumpUsed = true;
        jumpHeld = true;
        Debug.Log(coyoteFrames);
        Jump();
    }

    private void Jump()
    {
        currentJumpForce = jumpForce;
        var xSpeed = rigidBody.linearVelocityX - (movingPlatform is null ? 0 : rigidBody.linearVelocityX);
        currentJumpForce += rozbieg * Math.Abs(xSpeed);
        rigidBody.linearVelocityY = currentJumpForce;
        if (audioSource != null && jumpSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(jumpSound, AudioListener.volume);
            audioSource.pitch = 1f;
        }
    }

    private void HoldJump()
    {
        rigidBody.linearVelocityY = (float)(currentJumpForce * Math.Pow(jumpHoldFalloffExp, jumpHoldFrames));
    }


    private void CycleGroundedState()
    {
        wasGroundedLastFrame = isGrounded;
        isGrounded = IsGrounded();
    }

    private void DoGrapplePhysics()
    {
        if (queuedMovement.grappleExtend)
            if (grappleLength < grappleMaxLength)
                grappleLength += swingAcceleration;
        if (queuedMovement.grappleContract)
            if (grappleLength > 0.01f)
                grappleLength -= swingAcceleration;

        if (queuedMovement.grappleDown && enableMovement)
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
                if (dist > grappleGrabMaxLength) return;

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

        if (queuedMovement.grappleUp && enableMovement)
        {
            RemoveGrappling();
        }

        return;
    }

    void RemoveGrappling()
    {
        rigidBody.linearDamping = 0;
        isGrappling = false;
        distanceJoint.enabled = false;
        rope.enabled = false;
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
            gameObject.transform.SetParent(originalParent.transform);
            rigidBody.gravityScale = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("MovingPlatform"))
        {
            isOnMovingPlatform = true;
            gameObject.transform.SetParent(col.transform);
            rigidBody.gravityScale = 1.5f;
        }

        if (col.CompareTag("Heart"))
        {
            if (audioSource != null && bonusSound != null)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(bonusSound, AudioListener.volume);
                audioSource.pitch = 1f;
            }

            GameManager.instance.AddLives(1);
            Destroy(col.gameObject);
        }

        if (col.CompareTag("Bonus"))
        {
            if (audioSource != null && bonusSound != null)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(bonusSound, AudioListener.volume);
                audioSource.pitch = 1f;
            }

            GameManager.instance.AddPoints(2);
            Destroy(col.gameObject);
        }

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
        RemoveGrappling();
        animator.SetBool("isDead", true);
        GameManager.instance.AddLives(-1);
        if (audioSource != null && deathSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(deathSound, AudioListener.volume);
            audioSource.pitch = 1f;
        }

        StartCoroutine(GameManager.instance.RespawnSequence(this));
    }
}