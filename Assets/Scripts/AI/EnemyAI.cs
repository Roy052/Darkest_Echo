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
    const float ChangeTime = 1;

    public SpriteRenderer spriteRenderer;

    public EnemyType enemyType;

    public Vector2 targetPos;
    
    public float speed = 5f;
    public float detectionRadius = 10f;
    public float smoothTime = 0.1f;
    public string targetTag;
    public bool isSneak = false;
    public bool isFinding = false;

    protected List<Vector2> path = new List<Vector2>();
    protected int currentWaypoint = 0;
    protected float currentTime = 0;
    protected float currentSoundWaveTime = 0;

    protected UnityAction findPath;
    protected UnityAction funcEnter;

    protected Pathfinding pathfinding;

    bool isEntered = false;
    bool isWater = false;

    List<Vector2> soundWaveStorage = new List<Vector2>();
    AudioSource audioSource;

    public virtual void Start()
    {
        pathfinding = Singleton.pathFinding;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.color = new Color(1, 1, 1, enemyType == EnemyType.Scout ? 0 : 1);
        SetEnemy();
    }

    public virtual void Update()
    {
        if(enemyType == EnemyType.Fugitive && currentSoundWaveTime >= 1)
        {
            if(isWater)
                SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Wading,
                        transform.position, Color.blue);
            else if (isSneak)
                SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Sneaking,
                        transform.position);
            else
                SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal,
                        transform.position);
            currentSoundWaveTime = 0;
        }

        currentSoundWaveTime += Time.deltaTime;

        if (isFinding == false)
        {
            if (enemyType == EnemyType.Scout)
            {
                isScout = true;
                if (enemyType == EnemyType.Scout && (path == null || currentWaypoint >= path.Count))
                    findPath?.Invoke();

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
            else
                return;
        }
        else
        {
            isScout = false;

            if (path == null || path.Count == 0)
            {
                findPath?.Invoke();
                Debug.Log($"Target Pos : ({targetPos.x}, {targetPos.y})");
            }

            //If No Path
            if(path == null || path.Count == 0 || currentWaypoint >= path.Count)
            {
                isFinding = false;
                if(enemyType != EnemyType.Scout)
                    spriteRenderer.color = new Color(1, 1, 1, 0);

                if(audioSource)
                    audioSource.Stop();
                Debug.Log("No Path");
            }

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

        spriteRenderer.color = new Color(1, 1, 1, 0);

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
                spriteRenderer.color = new Color(1, 1, 1, 1);
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
            if (enemyType == EnemyType.Fugitive) return;

            targetPos = collision.GetComponent<SoundWave>().originPos;
            isFinding = true;
            audioSource.Play();
            spriteRenderer.color = new Color(1, 1, 1, 1);
            path = null;
        }

        if(collision.tag == Str.TagWater)
        {
            isWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isWater = false;   
    }

    public void MoveFugitive(Vector2 pos)
    {
        targetPos = pos;
        currentWaypoint = 0;
        currentTime = 0;
        path = null;
        isFinding = true;
    }
}