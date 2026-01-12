using System.Linq;
using UnityEngine;

public class FerrisWheelController : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;

    public int numberOfPlatforms = 5;

    public float Radius = 1;

    private GameObject[] platforms;
    private Rigidbody2D[] rb;

    private Vector2[] positions;

    [SerializeField] private float rotationSpeed = 0.5f;

    private float currentPhase = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        positions = new Vector2[numberOfPlatforms];
        GeneratePositions(0);

        rb = new Rigidbody2D[numberOfPlatforms];
        
        platforms = Enumerable.Range(0, numberOfPlatforms).Select(i =>
        {
            var platform = Instantiate(platformPrefab, positions[i], Quaternion.identity);
            platform.GetComponent<SpriteRenderer>().sortingLayerName = "Platforms";
            rb[i] = platform.GetComponent<Rigidbody2D>();
            return platform;
        }).ToArray();
    }
    
    // Update is called once per frame
    void Update()
    {
        currentPhase += Time.deltaTime * rotationSpeed;
        GeneratePositions(currentPhase);
    }
    
    private void FixedUpdate()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            var moveDirection = (positions[i] - (Vector2)transform.position).normalized;
            moveDirection = new Vector2(-moveDirection.y, moveDirection.x);
            Debug.DrawRay(platforms[i].transform.position, moveDirection, Color.red);
            rb[i].linearVelocity = moveDirection * rotationSpeed;
        }
    }

    void GeneratePositions(float phase)
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            var angle = (2 * Mathf.PI / numberOfPlatforms) * i + phase;
            var x = Mathf.Cos(angle) * Radius + transform.position.x;
            var y = Mathf.Sin(angle) * Radius + transform.position.y;
            positions[i] = new Vector2(x, y);
        }
    }
}