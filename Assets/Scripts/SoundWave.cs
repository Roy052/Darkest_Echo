using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private Vector3 reflectDir;
    public Vector3 moveDir;
    private float moveSpeed;
    private RaycastHit2D[] hitInfo;
    private Rigidbody2D rigid;
    private TrailRenderer trailRenderer;
    private float trailStartTime;
    public float fadeDuration;
    public Color originalColor;

    private void Start()
    {
        moveSpeed = 7f;
        moveDir = moveDir.normalized;
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[1];
        trailRenderer = GetComponent<TrailRenderer>();
        originalColor = trailRenderer.startColor;
        fadeDuration = 1.5f;
        trailStartTime = Time.time;
    }

    private void Update()
    {
        // Cast a ray to detect the wall
        rigid.Cast(moveDir, hitInfo, 0.1f);
        if (hitInfo[0].collider != null)
            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);

        // Fading sound wave and destroy it
        var elapsed = Time.time - trailStartTime;
        if (elapsed < fadeDuration)
        {
            var t = elapsed / fadeDuration;
            trailRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            trailRenderer.endColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
        }
        else
        {
            Destroy(gameObject);
        }
            
        // Move the sound wave
        transform.position += moveSpeed * Time.deltaTime * moveDir;
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
}