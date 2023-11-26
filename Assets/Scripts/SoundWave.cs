using System.Collections;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public bool isTemp = false;
    public bool isTempEndZone = false;
    public Vector2 originPos;

    private Vector3 reflectDir;
    private Vector3 moveDir;
    private Vector3 beforeMoveDir;
    private float moveSpeed;
    private RaycastHit2D[] hitInfo;
    private Rigidbody2D rigid;
    private TrailRenderer trailRenderer;
    private float trailStartTime;
    private float fadeDuration;
    private Color originalColor;
    private GameObject player;
    private bool isSneaking;
    private bool isClapping;
    private ContactFilter2D wallFilter;

    private readonly Color normalStartColor = new(1f, 1f, 1f, 1f);
    private readonly Color normalEndColor = new(1f, 1f, 1f, 0f);
    private readonly Color sneakingStartColor = new(1f, 1f, 1f, 0.5f);
    private readonly Color dyingStartColor = new(1f, 0f, 0f, 1f);
    private readonly Color dyingEndColor = new(1f, 0f, 0f, 0f);

    private void Awake()
    {
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[5];
        wallFilter = new ContactFilter2D();
        wallFilter.SetLayerMask(LayerMask.GetMask("Wall"));
        trailRenderer = GetComponent<TrailRenderer>();
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
        int result = rigid.Cast(moveDir, wallFilter, hitInfo);
       /*if(result >= 2 && hitInfo[1].collider != null)
            reflectDir = new Vector2(-moveDir.x, -moveDir.y);
        else*/ if (hitInfo[0].collider != null)
            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);
        Debug.DrawRay(transform.position, moveDir);

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
            trailRenderer.startColor =
                new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
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
        reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);
        reflectDir = reflectDir.normalized;
        beforeMoveDir = moveDir;
        moveDir = reflectDir;
        hitInfo = new RaycastHit2D[2];
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

            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);
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

    public void ChangeColor(Color color)
    {
        originalColor = color;
    }

    private IEnumerator DelayedStep()
    {
        yield return new WaitForSeconds(0.1f);
        trailRenderer.startColor = normalEndColor;
        SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal, transform.position);
    }
}