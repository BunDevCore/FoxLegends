using System;
using System.Linq;
using UnityEngine;

public class FerrisWheelController : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;

    public int numberOfPlatforms = 5;

    public float radius = 1;

    private class Platform
    {
        public readonly GameObject platformObject;

        public int currentWaypointIndex;

        // public Vector2 moveDirection;
        public float elapsedTime = 0;
        public Vector2 velocity;

        public Platform(GameObject platformObject, Vector2 velocity, int startWaypointIndex)
        {
            this.platformObject = platformObject;
            currentWaypointIndex = startWaypointIndex;
            this.velocity = velocity;
        }
    }

    private Platform[] platforms;

    private Vector2[] positions;

    [SerializeField] private float rotationSpeed = 0.5f;

    // private float currentPhase = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        positions = new Vector2[numberOfPlatforms];
        GeneratePositions();

        platforms = Enumerable.Range(0, numberOfPlatforms).Select(i =>
        {
            var platform = Instantiate(platformPrefab, positions[i], Quaternion.identity);
            var velocity = (positions[i] - (Vector2)transform.position).normalized * rotationSpeed;
            velocity = new Vector2(-velocity.y, velocity.x);
            return new Platform(platform, velocity, i);
        }).ToArray();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            platforms[i].elapsedTime += Time.deltaTime;
            float dist = Vector2.Distance(positions[platforms[i].currentWaypointIndex],
                positions[(platforms[i].currentWaypointIndex + 1) % positions.Length]);
            float percentageComplete = platforms[i].elapsedTime * Math.Abs(rotationSpeed) / dist;

            platforms[i].platformObject.transform.position = Vector3.Slerp(
                (Vector3)positions[
                    rotationSpeed > 0
                        ? platforms[i].currentWaypointIndex
                        : (platforms[i].currentWaypointIndex + 1) % positions.Length
                ] - transform.position,
                (Vector3)positions[
                    rotationSpeed > 0
                        ? (platforms[i].currentWaypointIndex + 1) % positions.Length
                        : platforms[i].currentWaypointIndex
                ] - transform.position,
                percentageComplete
            ) + transform.position;

            Vector2 velocity = (platforms[i].platformObject.transform.position - transform.position).normalized *
                               rotationSpeed;
            platforms[i].velocity = new Vector2(-velocity.y, velocity.x);

            Debug.DrawRay(platforms[i].platformObject.transform.position, platforms[i].velocity, Color.red);

            if (percentageComplete > 1)
            {
                platforms[i].elapsedTime = 0;
                platforms[i].currentWaypointIndex =
                    (platforms[i].currentWaypointIndex + (rotationSpeed > 0 ? 1 : positions.Length - 1)) %
                    positions.Length;
            }
        }
    }

    void GeneratePositions(float phase = 0)
    {
        for (int i = 0; i < numberOfPlatforms; i++)
        {
            var angle = (2 * Mathf.PI / numberOfPlatforms) * i + phase;
            var x = Mathf.Cos(angle) * radius + transform.position.x;
            var y = Mathf.Sin(angle) * radius + transform.position.y;
            positions[i] = new Vector2(x, y);
        }
    }
}