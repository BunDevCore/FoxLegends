using System.Linq;
using UnityEngine;

public class FerrisWheelController : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;

    public const int NUM_PLATFORMS = 5;

    public float Radius = 1;

    private GameObject[] platforms;
    private Rigidbody2D[] rb;

    private Vector2[] positions;

    [SerializeField] private float rotSpeed = 0.5f;

    private float currentPhase = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        positions = new Vector2[NUM_PLATFORMS];
        GeneratePositions(0);

        rb = new Rigidbody2D[NUM_PLATFORMS];
        
        platforms = Enumerable.Range(0, NUM_PLATFORMS).Select(i =>
        {
            var platform = Instantiate(platformPrefab, positions[i], Quaternion.identity);
            rb[i] = platform.GetComponent<Rigidbody2D>();
            return platform;
        }).ToArray();
    }
    
    // Update is called once per frame
    void Update()
    {
        currentPhase += Time.deltaTime * rotSpeed;
        GeneratePositions(currentPhase);
    }
    
    private void FixedUpdate()
    {
        for (int i = 0; i < NUM_PLATFORMS; i++)
        {
            var moveDirection = (Vector2)(platforms[i].transform.position - transform.position).normalized;
            moveDirection = new Vector2(-moveDirection.y, moveDirection.x);
            Debug.DrawRay(platforms[i].transform.position, moveDirection, Color.red);
            rb[i].linearVelocity = moveDirection * rotSpeed;
        }
    }

    void GeneratePositions(float phase)
    {
        for (int i = 0; i < NUM_PLATFORMS; i++)
        {
            var angle = (2 * Mathf.PI / NUM_PLATFORMS) * i + phase;
            var x = Mathf.Cos(angle) * Radius + transform.position.x;
            var y = Mathf.Sin(angle) * Radius + transform.position.y;
            positions[i] = new Vector2(x, y);
        }
    }
}