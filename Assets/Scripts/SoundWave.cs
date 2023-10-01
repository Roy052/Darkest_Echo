using System.Collections;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public Vector2 originPos;

    private Vector3 reflectDir;
    private Vector3 moveDir;
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

    private bool isReflecting = false;
    private void Awake()
    {
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[2];
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
        //if (isReflecting) return;
        int result = rigid.Cast(moveDir, wallFilter, hitInfo);
        if(result >= 2 && hitInfo[1].collider != null)
            reflectDir = new Vector2(-moveDir.x, -moveDir.y);
        else if (hitInfo[0].collider != null)
            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);

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
        }
        else
        {
            SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Move the sound wave
        transform.position += moveSpeed * Time.deltaTime * moveDir;
        trailStartTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the sound wave trigger with a wall, it will reflect
        if (!other.gameObject.CompareTag("Wall")) return;
        //if (isReflecting) return;
        isReflecting = true;
        reflectDir = reflectDir.normalized;
        moveDir = reflectDir;
        hitInfo = new RaycastHit2D[2];
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Wall")) return;
        isReflecting = false;
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
                moveSpeed = 8f;
                fadeDuration = 1.5f;
                trailRenderer.time = 1f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 1: // Sneaking
                moveSpeed = 6f;
                fadeDuration = 0.6f;
                trailRenderer.time = 0.55f;
                trailRenderer.startColor = sneakingStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 2: // Clapping
                moveSpeed = 8f;
                float tempValue = player.GetComponentInChildren<Clap>().clapPower;
                fadeDuration = tempValue == 0 ? 1 : tempValue;
                trailRenderer.time = fadeDuration * 0.7f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 3: // Diving
                moveSpeed = 8f;
                fadeDuration = 2f;
                trailRenderer.time = 1.8f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 4: // Death
                moveSpeed = 8f;
                fadeDuration = 10f;
                trailRenderer.time = 1.5f;
                trailRenderer.startColor = dyingStartColor;
                trailRenderer.endColor = dyingEndColor;
                break;
            case 5: // Throwing
                moveSpeed = 12f;
                fadeDuration = 2.5f;
                trailRenderer.time = 2.4f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
            case 6:
                moveSpeed = 3f;
                fadeDuration = 3600f;
                trailRenderer.time = 1f;
                trailRenderer.startColor = normalStartColor;
                trailRenderer.endColor = normalEndColor;
                break;
        }
    }

    private IEnumerator DelayedStep()
    {
        yield return new WaitForSeconds(0.1f);
        trailRenderer.startColor = normalEndColor;
        SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal, transform.position);
    }
}