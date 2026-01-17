using Unity.Mathematics.Geometry;
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

    [SerializeField] private GameObject platformPrefab;
    private GameObject platform;

    private int currentWaypointIndex = 0;

    [SerializeField] private float speed = 1f;

    private float elapsedTime;
    public Vector2 velocity;

    private void Awake()
    {
        platform = Instantiate(platformPrefab, waypoints[currentWaypointIndex].transform.position, Quaternion.identity);
        velocity =
            (waypoints[(currentWaypointIndex + 1) % waypoints.Length].transform.position -
             waypoints[currentWaypointIndex].transform.position).normalized * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        float dist = Vector2.Distance(
            waypoints[currentWaypointIndex].transform.position,
            waypoints[(currentWaypointIndex + 1) % waypoints.Length].transform.position);
        float percentageComplete = elapsedTime * speed / dist;

        platform.transform.position = Vector2.Lerp(
            waypoints[currentWaypointIndex].transform.position,
            waypoints[(currentWaypointIndex + 1) % waypoints.Length].transform.position,
            percentageComplete
        );

        if (percentageComplete > 1f)
        {
            elapsedTime = 0;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            velocity =
                (waypoints[(currentWaypointIndex + 1) % waypoints.Length].transform.position -
                 waypoints[currentWaypointIndex].transform.position).normalized * speed;
        }
        
        Debug.DrawRay(platform.transform.position, velocity, Color.green);
        
    }
}