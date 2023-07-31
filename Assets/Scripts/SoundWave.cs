using UnityEngine;
using UnityEngine.Pool;

public class SoundWave : MonoBehaviour
{
    private Vector3 reflectDir;
    private Vector3 moveDir;
    private float moveSpeed;
    private RaycastHit2D[] hitInfo;
    private Rigidbody2D rigid;
    private TrailRenderer trailRenderer;
    private float trailStartTime;
    public float fadeDuration;
    private Color originalColor;
    private GameObject player;
    private bool isSneaking;
    
    private void Awake()
    {
        moveSpeed = 7f;
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[1];
        trailRenderer = GetComponent<TrailRenderer>();
        fadeDuration = 1.5f;
        trailRenderer.time = 1f;
        trailStartTime = Time.time;
        player = GameObject.Find("Player");

        isSneaking = player.GetComponent<PlayerMovement>().IsSneaking();
        // Player sneaking logic
        if (isSneaking)
        {
            trailRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            moveSpeed = 5f;
            trailRenderer.time = 0.5f;
            fadeDuration = 1f;
        }

        originalColor = trailRenderer.startColor;
    }

    float currentTime = 0;
    private void Update()
    {
        // Cast a ray to detect the wall
        rigid.Cast(moveDir, hitInfo);
        if (hitInfo[0].collider != null)
            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);

        // Fading sound wave and destroy it
        if (currentTime < fadeDuration)
        {
            var t = currentTime / fadeDuration;
            trailRenderer.startColor =
                new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
        }
        else
        {
            SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
        }

        // Move the sound wave
        transform.position += moveSpeed * Time.deltaTime * moveDir;
        currentTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the sound wave trigger with a wall, it will reflect
        if (other.gameObject.CompareTag("Wall"))
        {
            moveDir = reflectDir.normalized;
            hitInfo = new RaycastHit2D[1];
        }
    }

    public void SetMoveDir(Vector3 dir)
    {
        moveDir = dir.normalized;
    }

    public void SetDuration(float fade, float trail)
    {
        fadeDuration = fade;
        trailRenderer.time = trail;
    }
}