using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public bool isTemp = false;
    public bool isTempEndZone = false;
    public Vector2 originPos;

    public SpriteRenderer spriteRenderer;
    public TrailRenderer trailRenderer;
    public Rigidbody2D rigid;
    public CircleCollider2D circleCollider;

    private Vector3 reflectDir;
    private Vector3 moveDir;
    private Vector3 beforeMoveDir;
    private float moveSpeed;
    private RaycastHit2D[] hitInfo;
    private float trailStartTime;
    private float fadeDuration;
    private Color originalColor;
    private GameObject player;
    private bool isSneaking;
    private bool isClapping;
    private bool isThrowing;
    private ContactFilter2D wallFilter;

    private readonly Color normalStartColor = new(1f, 1f, 1f, 1f);
    private readonly Color normalEndColor = new(1f, 1f, 1f, 0f);
    private readonly Color sneakingStartColor = new(1f, 1f, 1f, 0.5f);
    private readonly Color dyingStartColor = new(1f, 0f, 0f, 1f);
    private readonly Color dyingEndColor = new(1f, 0f, 0f, 0f);

    private float createTime = 0f;

    private void Awake()
    {
        reflectDir = Vector3.zero;
        hitInfo = new RaycastHit2D[5];
        wallFilter = new ContactFilter2D();
        wallFilter.SetLayerMask(LayerMask.GetMask("Wall"));
        player = GameObject.Find("Player");
        SetType(0);
    }

    private void OnEnable()
    {
        trailStartTime = 0;
        originalColor = trailRenderer.startColor;
    }

    private void Update()
    {
        // Cast a ray to detect the wall
        if (circleCollider.isTrigger)
        {
            int count = rigid.Cast(moveDir, wallFilter, hitInfo);
            if (count > 0)
            {
                if (hitInfo[0].collider != null)
                    reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal).normalized;
                Debug.DrawRay(transform.position, moveDir);
            }
        }

        if (hitInfo[0].distance < 0.1f && Mathf.Approximately(moveSpeed, 12f))
        {
            moveSpeed = 0f;
            moveDir = Vector3.zero;
            trailRenderer.startColor = normalEndColor;
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Clapping, transform.position);
        }

        // Fading sound wave and destroy it
        if (trailStartTime < fadeDuration)
        {
            var t = trailStartTime / fadeDuration;
            Color fadedColor =
                new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            trailRenderer.startColor = fadedColor;
            spriteRenderer.color = fadedColor;
        }
        else if (fadeDuration == 0)
        {
            // Do nothing
            if(isTemp == false)
                ChangeColor(Color.white);
        }
        else
        {
            if (isTemp == false)
                SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
            else
                Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Move the sound wave
        transform.position += moveSpeed * Time.deltaTime * moveDir;
        trailStartTime += Time.deltaTime;
    }

    public int colliderCount = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // When Touch Wall, Split
        if (isTemp == false && (other.gameObject.CompareTag(Str.TagTrap) || other.gameObject.CompareTag(Str.TagWater)))
        {
            GameObject temp = Instantiate(gameObject, transform.parent);
            SoundWave tempSoundwave = temp.GetComponent<SoundWave>();
            tempSoundwave.isTemp = true;
            tempSoundwave.ChangeColor(other.gameObject.GetComponent<Obstacles>().color);
            tempSoundwave.moveDir = moveDir;
            temp.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        else if (isTemp == false && other.gameObject.CompareTag(Str.TagEndZone))
        {
            GameObject temp = Instantiate(gameObject, transform.parent);
            SoundWave tempSoundwave = temp.GetComponent<SoundWave>();
            tempSoundwave.isTemp = true;
            tempSoundwave.isTempEndZone = true;
            tempSoundwave.moveDir = moveDir;
            tempSoundwave.trailRenderer.startWidth *= 2;
            temp.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }

        // When the sound wave trigger with a wall, it will reflect
        if (!other.gameObject.CompareTag(Str.TagWall)) return;

        if (isTemp == false && isThrowing)
            StartCoroutine(DelayedStep());

        reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal).normalized;
        beforeMoveDir = moveDir;
        moveDir = reflectDir;
        hitInfo = new RaycastHit2D[5];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When Touch Wall, Split
        if (isTemp == false && (collision.gameObject.CompareTag(Str.TagTrap) || collision.gameObject.CompareTag(Str.TagWater)))
        {
            GameObject temp = Instantiate(gameObject, transform.parent);
            SoundWave tempSoundwave = temp.GetComponent<SoundWave>();
            tempSoundwave.isTemp = true;
            tempSoundwave.ChangeColor(collision.gameObject.GetComponent<Obstacles>().color);
            tempSoundwave.moveDir = moveDir;
            temp.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        else if (isTemp == false && collision.gameObject.CompareTag(Str.TagEndZone))
        {
            GameObject temp = Instantiate(gameObject, transform.parent);
            SoundWave tempSoundwave = temp.GetComponent<SoundWave>();
            tempSoundwave.isTemp = true;
            tempSoundwave.isTempEndZone = true;
            tempSoundwave.moveDir = moveDir;
            tempSoundwave.trailRenderer.startWidth *= 2;
            temp.GetComponent<SpriteRenderer>().sortingOrder = 10;
            tempSoundwave.circleCollider.isTrigger = true;
            circleCollider.isTrigger = true;
        }

        // When the sound wave trigger with a wall, it will reflect
        if (!collision.gameObject.CompareTag(Str.TagWall)) return;

        if (isTemp == false && isThrowing)
            StartCoroutine(DelayedStep());

        Vector2 normal = collision.contacts[0].normal;
        reflectDir = Vector2.Reflect(moveDir, normal).normalized;
        beforeMoveDir = moveDir;
        moveDir = reflectDir;
        hitInfo = new RaycastHit2D[5];
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If Temp SoundWave, Destroy At Exit2D
        if (isTemp && (collision.gameObject.CompareTag(Str.TagTrap) || collision.gameObject.CompareTag(Str.TagWater)))
            Destroy(gameObject);
        else if (collision.gameObject.CompareTag(Str.TagEndZone))
        {
            if (isTempEndZone == false)
            {
                trailStartTime = fadeDuration;
                return;
            }

            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);
            reflectDir = reflectDir.normalized;
            beforeMoveDir = moveDir;
            moveDir = reflectDir;
            hitInfo = new RaycastHit2D[2];
        }
        if (!collision.gameObject.CompareTag(Str.TagWall)) return;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If Temp SoundWave, Destroy At Exit2D
        if (isTemp && (other.gameObject.CompareTag(Str.TagTrap) || other.gameObject.CompareTag(Str.TagWater)))
            Destroy(gameObject);
        else if (other.gameObject.CompareTag(Str.TagEndZone))
        {
            if (isTempEndZone == false)
            {
                trailStartTime = fadeDuration;
                return;
            }

            Vector2 normal = Vector2.zero;
            BoxCollider2D boxCollider = other.gameObject.GetComponent<BoxCollider2D>();
            Bounds b = boxCollider.bounds;
            Vector2 minBounds = b.min; // (왼쪽 아래)
            Vector2 maxBounds = b.max; // (오른쪽 위)

            //Top
            if(transform.position.y > maxBounds.y)
            {
                normal = Vector2.up;
            }

            //Bottom
            if (transform.position.y < minBounds.y)
            {
                normal = Vector2.down;
            }

            //Left
            if (transform.position.x < minBounds.x)
            {
                normal = Vector2.left;
            }

            //Right
            if (transform.position.x > maxBounds.x)
            {
                normal = Vector2.right;
            }


            reflectDir = Vector2.Reflect(moveDir, normal);
            reflectDir = reflectDir.normalized;
            beforeMoveDir = moveDir;
            moveDir = reflectDir;
            hitInfo = new RaycastHit2D[2];
        }
        if (!other.gameObject.CompareTag(Str.TagWall)) return;
    }

    public void SetMoveDir(Vector3 dir)
    {
        moveDir = dir.normalized;
    }

    public void SetType(SoundWaveGenerator.WaveType type)
    {
        isThrowing = false;
        switch ((int)type)
        {
            case 0: // Normal
                moveSpeed = 5f;
                fadeDuration = 2.4f;
                trailRenderer.time = 1f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 1: // Sneaking
                moveSpeed = 5f;
                fadeDuration = 0.6f;
                trailRenderer.time = 0.55f;
                trailRenderer.startColor = sneakingStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 2: // Clapping
                moveSpeed = 5f;
                float tempValue = player.GetComponentInChildren<Clap>().clapPower;
                fadeDuration = tempValue == 0 ? 3 : 1.5f * tempValue;
                trailRenderer.time = fadeDuration * 0.7f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 3: // Diving
                moveSpeed = 5f;
                fadeDuration = 5f;
                trailRenderer.time = 1.8f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 4: // Death
                moveSpeed = 5f;
                fadeDuration = 10f;
                trailRenderer.time = 1.5f;
                trailRenderer.startColor = dyingStartColor;
                trailRenderer.endColor = dyingEndColor;
                break;
            case 5: // Throwing
                moveSpeed = 11f;
                fadeDuration = 3f;
                trailRenderer.time = 2.4f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                isThrowing = true;
                break;
            case 6: //Eternal
                moveSpeed = 1f;
                fadeDuration = 3600f;
                trailRenderer.time = 1f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
        }
    }

    public void SetCreateTime(float time)
    {
        createTime = time;
    }

    public float GetCreateTime()
    {
        return createTime;
    }

    public void ChangeColor(Color color)
    {
        originalColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);
    }

    public void ClearSoundWave()
    {
        trailRenderer.Clear();
        ChangeColor(Color.white);
        circleCollider.isTrigger = false;
    }

    WaitForSeconds waitForOneTenthSeconds = new WaitForSeconds(0.1f);
    private IEnumerator DelayedStep()
    {
        yield return waitForOneTenthSeconds;
        trailRenderer.startColor = normalEndColor;
        SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal, transform.position);
        if(isTemp == false)
            SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
        else
            Destroy(gameObject);
    }
}