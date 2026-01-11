using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BunnyController : MonoBehaviour
{
    [Header("Bunny settings")] [SerializeField]
    private float moveSpeed = 1.5f;

    [SerializeField] private float moveRange = 1.0f;
    [SerializeField] private float minWaitTime = 1.0f;
    [SerializeField] private float maxWaitTime = 4.0f;
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private Transform graphicsTransform;
    [SerializeField] private Animator graphicsAnimator;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isFacingRight = true;
    private bool isPlayerInRange;
    
    [Header("Interaction Event")]
    public UnityEvent onInteract;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void Start()
    {
        buttonObject.SetActive(false);
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Collider2D bunnyCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(bunnyCollider, playerCollider);
        }

        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            if (graphicsAnimator) graphicsAnimator.SetBool("isRunning", false);
            yield return new WaitForSeconds(waitTime);
            float targetX = Random.Range(startPosition.x - moveRange, startPosition.x + moveRange);
            yield return StartCoroutine(MoveToX(targetX));
        }
    }

    IEnumerator MoveToX(float targetX)
    {
        if (graphicsAnimator) graphicsAnimator.SetBool("isRunning", true);
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            if (DialogueManager.instance && DialogueManager.instance.IsActive)
            {
                rb.linearVelocity = Vector2.zero;
                yield return null;
                continue;
            }

            float direction = targetX > transform.position.x ? 1 : -1;
            if (direction > 0 && !isFacingRight) Flip();
            else if (direction < 0 && isFacingRight) Flip();
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = graphicsTransform.localScale;
        theScale.x *= -1;
        graphicsTransform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (buttonObject != null) buttonObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (buttonObject != null) buttonObject.SetActive(false);
        }
    }

    private void Interact()
    {
        if (!DialogueManager.instance || DialogueManager.instance.IsActive) return;

        onInteract?.Invoke();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(startPosition, Vector2.left * moveRange);
        Gizmos.DrawRay(startPosition, Vector2.right * moveRange);
    }
}