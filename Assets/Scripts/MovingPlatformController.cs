using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Header("Movement parameters")] [Range(0.01f, 20.0f)] [SerializeField]
    private float moveSpeed = 0.1f;
    private float startPositionX;
    [SerializeField]
    private float moveRange = 1.0f;

    private bool isMovingRight = false;
    void Awake()
    {
        startPositionX = transform.position.x;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
        {
            if (this.transform.position.x < startPositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                isMovingRight = false;
            }
        }
        else
        {
            if (this.transform.position.x > startPositionX)
            {
                MoveLeft();
            }
            else
            {
                isMovingRight = true;
            }
        }
    }

    void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
    
    void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
}
