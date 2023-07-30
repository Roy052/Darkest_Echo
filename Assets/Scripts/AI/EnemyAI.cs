using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : Singleton
{
    readonly float ChangeTime = 1;

    public Transform player;
    public float speed = 5f;
    public float detectionRadius = 10f;
    public float smoothTime = 0.1f;

    private Pathfinding pathfinding;
    private List<Vector2> path;
    private int currentWaypoint = 0;
    private float currentTime = 0;

    private void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    private void Update()
    {
        // Check if the player is within the detection radius
        if (Vector2.Distance(transform.position, player.position) < detectionRadius)
        {
            if (path == null || currentWaypoint >= path.Count || currentTime >= ChangeTime)
            {
                // Calculate a new path to the player
                path = pathfinding.FindPath(transform.position, player.position);
                currentWaypoint = 0;
                currentTime = 0;
            }

            // Move towards the next waypoint on the path
            if (currentWaypoint < path.Count)
            {
                Vector2 direction = (path[currentWaypoint] - (Vector2)transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                //Vector2 targetPosition = path[currentWaypoint];
                //Vector2 newPosition = Vector2.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, speed);
                //transform.position = newPosition;

                // Check if the enemy has reached the current waypoint
                if (Vector2.Distance(transform.position, path[currentWaypoint]) < 0.1f)
                {
                    currentWaypoint++;
                }
            }
        }

        currentTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log("Catch");
            //stageSM.StageRestart();
        }
    }
}