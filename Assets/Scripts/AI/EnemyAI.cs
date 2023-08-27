using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum EnemyType
{
    None = 0,
    Fugitive = 1,
    Chase = 2,
    Scout = 3,
    Smasher = 4,
    Assassin = 5,
    Shooter = 6,
}

public class EnemyAI : MonoBehaviour
{
    const float FoundDetectRadius = 0.5f;

    readonly float ChangeTime = 1;

    public EnemyType enemyType;

    public Vector2 targetPos;
    public Pathfinding pathfinding;
    public float speed = 5f;
    public float detectionRadius = 10f;
    public float smoothTime = 0.1f;

    protected List<Vector2> path = new List<Vector2>();
    protected int currentWaypoint = 0;
    protected float currentTime = 0;

    protected string targetTag;
    protected UnityAction findPath;
    protected UnityAction funcEnter;

    protected bool isFinding;

    public virtual void Start()
    {
        isFinding = false;

        switch (enemyType)
        {
            case EnemyType.None:
                break;
            case EnemyType.Fugitive:
                findPath = Chase;
                if(Singleton.stageSM != null)
                    funcEnter = Singleton.stageSM.StageEnd ;
                break;
            case EnemyType.Chase:
                findPath = Chase;
                if (Singleton.stageSM != null)
                    funcEnter = Singleton.stageSM.StageRestart;
                break;
            case EnemyType.Scout:
                isScout = true;
                findPath = Scout;
                if (Singleton.stageSM != null)
                    funcEnter = Singleton.stageSM.StageRestart;
                break;
            case EnemyType.Smasher:
                findPath = SmashChase;
                if (Singleton.stageSM != null)
                    funcEnter = Singleton.stageSM.StageRestart;
                break;
            default:
                break;
        }
    }

    public virtual void Update()
    {
        if (isFinding == false)
        {
            if (enemyType == EnemyType.Scout)
            {
                isScout = true;
                if (enemyType == EnemyType.Scout && (path == null || currentWaypoint >= path.Count))
                    findPath?.Invoke();
            }
            else
                return;
        }
        else
        {
            isScout = false;

            if (path == null || currentWaypoint >= path.Count || currentTime >= ChangeTime)
            {
                findPath?.Invoke();
            }

            //If No Path
            if (path == null) return;

            // Move towards the next waypoint on the path
            if (path != null && currentWaypoint < path.Count)
            {
                Vector2 direction = (path[currentWaypoint] - (Vector2)transform.position).normalized;
                transform.position += (Vector3)direction * speed * Time.deltaTime;

                // Check if the enemy has reached the current waypoint
                if (Vector2.Distance(transform.position, path[currentWaypoint]) < 0.1f)
                {
                    currentWaypoint++;
                }
            }
        }

        currentTime += Time.deltaTime;
    }

    protected void Chase()
    {
        // Calculate a new path to the player
        path = pathfinding.FindPath(transform.position, targetPos);
        currentWaypoint = 0;
        currentTime = 0;
    }

    public Vector2 pointA, pointB;
    bool isScout = false;
    void Scout()
    {
        if (isScout)
        {
            //A is close
            if (Vector2.Distance(transform.position, pointA) < Vector2.Distance(transform.position, pointB))
            {
                path = new List<Vector2>() { pointB };
                currentWaypoint = 0;
                currentTime = 0;
            }
            else
            {
                path = new List<Vector2>() { pointA };
                currentWaypoint = 0;
                currentTime = 0;
            }
        }
        else
        {
            Chase();
        }
    }

    void SmashChase()
    {
        path = new List<Vector2>() { targetPos };
        currentWaypoint = 0;
        currentTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == targetTag)
        {
            Debug.Log("Enter Target");
            funcEnter?.Invoke();
        }

        if(collision.tag == "SoundWave")
        {
            detectionRadius = FoundDetectRadius;
            targetPos = collision.GetComponent<SoundWave>().originPos;
            isFinding = true;
        }
    }
}