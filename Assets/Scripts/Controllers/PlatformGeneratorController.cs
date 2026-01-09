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
        public readonly Rigidbody2D rigidBody;
        public int waypointIndex;
        public Vector2 moveDirection;

        public Platform(GameObject platformObject, Rigidbody2D rigidBody, int waypointIndex)
        {
            this.platformObject = platformObject;
            this.rigidBody = rigidBody;
            this.waypointIndex = waypointIndex;
            moveDirection = Vector2.zero;
        }

        public void CalculateDirection(GameObject[] waypoints)
        {
            moveDirection = (waypoints[waypointIndex].transform.position - platformObject.transform.position)
                .normalized;
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
        platforms.Add(new Platform(platform, platform.GetComponent<Rigidbody2D>(), 1));
        platforms.Last().CalculateDirection(waypoints);
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
            var dist = Vector2.Distance(
                platforms[i].platformObject.transform.position,
                waypoints[platforms[i].waypointIndex].transform.position);
            if (dist < 0.1f)
            {
                platforms[i].waypointIndex += 1;

                if (platforms[i].waypointIndex < waypoints.Length)
                    platforms[i].CalculateDirection(waypoints);
                else
                {
                    Destroy(platforms[i].platformObject);
                    platforms.RemoveAt(i);
                }
            }
        }

        if (Time.time - lastSpawnTime >= interval)
        {
            CreatePlatform();
            lastSpawnTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        platforms.ForEach(platform => platform.rigidBody.linearVelocity = platform.moveDirection * speed);
    }
}