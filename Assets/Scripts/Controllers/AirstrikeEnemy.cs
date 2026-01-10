using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirstrikeEnemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Vector2 startPosition;
    private bool isFacingRight = true;
    private bool shouldMove = true;

    [Header("Enemy Settings")] [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField] private float maxStep = 2f;
    [SerializeField] private float maxMovementArea = 5f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 3f;

    [Header("Bomb Settings")] [SerializeField]
    private float bombTimer = 3f;

    [SerializeField] private GameObject bombPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Start()
    {
        StartCoroutine(WanderRoutine());
        StartCoroutine(Bombing());
    }

    IEnumerator Bombing()
    {
        while (shouldMove)
        {
            yield return new WaitForSeconds(bombTimer);
            if (bombPrefab)
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
        }
    }

    IEnumerator WanderRoutine()
    {
        while (shouldMove)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            if (playerTransform == null) continue;
            float distanceToPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);
            float targetX;
            if (distanceToPlayer < 0.1f)
            {
                float randomDir = Random.value > 0.5f ? 1f : -1f;
                targetX = transform.position.x + (randomDir * maxStep);
            }
            else
                targetX = playerTransform.position.x;

            targetX = Mathf.Clamp(targetX, startPosition.x - maxMovementArea, startPosition.x + maxMovementArea);
            targetX = Mathf.Clamp(targetX, transform.position.x - maxStep, transform.position.x + maxStep);

            yield return StartCoroutine(LerpToX(targetX));
        }
    }

    IEnumerator LerpToX(float targetX)
    {
        Vector2 startPos = rb.position;
        Vector2 endPos = new Vector2(targetX, rb.position.y);

        float distance = Mathf.Abs(startPos.x - targetX);
        if (distance < 0.05f) yield break;

        float duration = distance / moveSpeed;
        float elapsed = 0f;

        if (targetX > transform.position.x && isFacingRight) Flip();
        else if (targetX < transform.position.x && !isFacingRight) Flip();

        while (elapsed < duration)
        {
            if (DialogueManager.instance != null && DialogueManager.instance.IsActive)
            {
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, t));
            yield return null;
        }

        rb.MovePosition(endPos);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) startPosition = transform.position;

        Gizmos.color = Color.red;
        Vector3 leftBound = new Vector3(startPosition.x - maxMovementArea, transform.position.y, 0);
        Vector3 rightBound = new Vector3(startPosition.x + maxMovementArea, transform.position.y, 0);

        Gizmos.DrawLine(leftBound + Vector3.up, leftBound + Vector3.down);
        Gizmos.DrawLine(rightBound + Vector3.up, rightBound + Vector3.down);
        Gizmos.color = Color.cyan;
        if (playerTransform)
        {
            float pX = Math.Clamp(playerTransform.position.x, transform.position.x - maxStep,
                transform.position.x + maxStep);
            Vector3 startP = new Vector3(pX, transform.position.y, transform.position.z);
            Gizmos.DrawLine(transform.position, startP);
        }
    }

    public void Death() => shouldMove = false;
}