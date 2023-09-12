using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum EnemyType
{
    None = -1,
    Fugitive = 0,
    Chase = 1,
    Scout = 2,
    Smasher = 3,
    Assassin = 4,
    Shooter = 5,
}

public class EnemyAI : MonoBehaviour
{
    const float FoundDetectRadius = 3;

    readonly float ChangeTime = 1;

    public EnemyType enemyType;

    public Vector2 targetPos;
    
    public float speed = 5f;
    public float detectionRadius = 10f;
    public float smoothTime = 0.1f;
    public string targetTag;

    protected List<Vector2> path = new List<Vector2>();
    protected int currentWaypoint = 0;
    protected float currentTime = 0;

    protected UnityAction findPath;
    protected UnityAction funcEnter;

    protected bool isFinding = false;
    protected Pathfinding pathfinding;

    bool isEntered = false;
    List<Vector2> soundWaveStorage = new List<Vector2>();

    public virtual void Start()
    {
        pathfinding = Singleton.pathFinding;
        SetEnemy();
    }

    public virtual void Update()
    {
        if(enemyType == EnemyType.Fugitive && currentTime >= 1)
        {
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal,
                        transform.position);
        }

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

    public void SetEnemy()
    {
        path = new List<Vector2>();
        currentWaypoint = 0;
        currentTime = 0;
        isFinding = false;
        isEntered = false;

        switch (enemyType)
        {
            case EnemyType.None:
                break;
            case EnemyType.Fugitive:
                findPath = Chase;
                if (Singleton.stageSM != null)
                    funcEnter = Singleton.stageSM.StageEnd;
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
        if (collision.tag == targetTag && isEntered == false 
            && Vector2.Distance(transform.position, collision.transform.position) < FoundDetectRadius)
        {
            Debug.Log("Enter Target");
            funcEnter?.Invoke();
            isEntered = true;
        }

        if (collision.tag == "SoundWave")
        {
            if (Vector2.Distance(targetPos, collision.GetComponent<SoundWave>().originPos) < Singleton.RangeOfError) return;

            targetPos = collision.GetComponent<SoundWave>().originPos;
            isFinding = true;
            path = null;
        }
    }
}