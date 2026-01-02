using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField]
    private GameObject[] waypoints;

    private int currentWaypoint = 0;

    [SerializeField] private float speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dist = Vector2.Distance(transform.position, waypoints[currentWaypoint].transform.position);
        if (dist <= 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        var mt = Vector2.MoveTowards(this.transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);
        this.transform.position = mt;
    }
}
