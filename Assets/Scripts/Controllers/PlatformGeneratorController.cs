using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformGeneratorController : MonoBehaviour
{
    [Header("You need to have at least 2 waypoints")] [SerializeField]
    private GameObject[] waypoints;

    [SerializeField] private GameObject platformPrefab;

    private class Platform
    {
        public readonly GameObject platformObject;

        public int currentWaypointIndex;

        // public Vector2 moveDirection;
        public float elapsedTime = 0;
        public Vector2 velocity;

        public Platform(GameObject platformObject, Vector2 velocity)
        {
            this.platformObject = platformObject;
            currentWaypointIndex = 0;
            this.velocity = velocity;
        }
    }

    private List<Platform> platforms = new();

    [SerializeField] private float speed = 1f;

    [Header("Platforms spawn interval in seconds")] [SerializeField]
    private float interval = 3;

    private float lastSpawnTime;

    private void CreatePlatform()
    {
        var platform = Instantiate(platformPrefab, waypoints[0].transform.position, Quaternion.identity);
        Vector2 velocity = (waypoints[1].transform.position - waypoints[0].transform.position).normalized * speed;
        platforms.Add(new Platform(platform, velocity));
    }

    private void Start()
    {
        lastSpawnTime = Time.time - interval;
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = platforms.Count - 1; i >= 0; i--)
        {
            platforms[i].elapsedTime += Time.deltaTime;
            float dist = Vector2.Distance(
                waypoints[platforms[i].currentWaypointIndex].transform.position,
                waypoints[platforms[i].currentWaypointIndex + 1].transform.position);
            float percentageComplete = platforms[i].elapsedTime * speed / dist;

            platforms[i].platformObject.transform.position = Vector2.Lerp(
                waypoints[platforms[i].currentWaypointIndex].transform.position,
                waypoints[platforms[i].currentWaypointIndex + 1].transform.position,
                percentageComplete
            );

            if (percentageComplete > 1f)
            {
                platforms[i].elapsedTime = 0;
                platforms[i].currentWaypointIndex += 1;
                if (platforms[i].currentWaypointIndex == waypoints.Length - 1)
                {
                    Destroy(platforms[i].platformObject);
                    platforms.RemoveAt(i);
                    continue;
                }

                platforms[i].velocity =
                    (waypoints[(platforms[i].currentWaypointIndex + 1) % waypoints.Length].transform.position -
                     waypoints[platforms[i].currentWaypointIndex].transform.position).normalized * speed;
            }

            Debug.DrawRay(platforms[i].platformObject.transform.position, platforms[i].velocity, Color.green);
        }

        if (Time.time - lastSpawnTime >= interval)
        {
            CreatePlatform();
            lastSpawnTime = Time.time;
        }
    }
}