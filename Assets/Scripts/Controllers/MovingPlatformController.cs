using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Header("Żeby gracz przesuwał się z obiektem trzeba dodać \n" +
            "`BoxCollider2D`, włączyć w nim `is Trigger`\n" +
            "i ustawić go tak, aby lekko wystawał z góry.\n\n" +
            "Trzeba również ustawić Tag obiektowi na `MovingPlatform`\n" +
            "oraz ustawić `Layer` na `Ground`")]
    [SerializeField]
    private GameObject[] waypoints;

    [SerializeField] private GameObject platform;
    

    private int currentWaypointIndex = 0;

    [SerializeField] private float speed = 1f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake() => rb = platform.GetComponent<Rigidbody2D>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platform.transform.position = waypoints[currentWaypointIndex].transform.position;
        CalculateDirection();
    }

    // Update is called once per frame
    void Update()
    {
        var dist = Vector2.Distance(platform.transform.position, waypoints[currentWaypointIndex].transform.position);
        if (dist < 0.01f) currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        CalculateDirection();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    private void CalculateDirection()
    {
        moveDirection = (waypoints[currentWaypointIndex].transform.position - platform.transform.position).normalized;
    }
}