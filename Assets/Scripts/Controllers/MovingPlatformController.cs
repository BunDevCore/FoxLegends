using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Header("Żeby gracz przesuwał się z obiektem trzeba dodać \n" +
            "`BoxCollider2D`, włączyć w nim `is Trigger`\n" +
            "i ustawić go tak, aby lekko wystawał z góry.\n\n" +
            "Trzeba również ustawić Tag obiektowi na `MovingPlatform`")]
    [SerializeField] private GameObject[] waypoints;

    private int currentWaypointIndex = 0;

    [SerializeField] private float speed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = waypoints[currentWaypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var dist = Vector2.Distance(transform.position, waypoints[currentWaypointIndex].transform.position);
        if (dist < 0.1f) currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        transform.position = Vector2.MoveTowards(
            transform.position,
            waypoints[currentWaypointIndex].transform.position,
            speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        
    }
}