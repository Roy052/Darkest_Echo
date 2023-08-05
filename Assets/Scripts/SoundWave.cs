using System;
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
    private Renderer soundWaveRenderer;
    private float trailStartTime;
    private float fadeDuration;
    private Color originalColor;
    private GameObject player;
    private float currentTime;
    private bool isSneaking;
    private bool isClapping;
    
    
    private void Awake()
    {
        currentTime = 0;
        moveSpeed = 7f;
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[1];
        trailRenderer = GetComponent<TrailRenderer>();
        soundWaveRenderer = GetComponent<Renderer>();
        fadeDuration = 1.5f;
        trailRenderer.time = 1f;
        player = GameObject.Find("Player"); 
    }

    private void OnEnable()
    {
        trailStartTime = Time.time;
        
        // Player sneaking logic
        if (player.GetComponent<PlayerMovement>().isSneaking)
        {
            var sneakingColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            trailRenderer.startColor = sneakingColor;
            soundWaveRenderer.material.color = sneakingColor;
            moveSpeed = 4f;
            trailRenderer.time = 0.6f;
            fadeDuration = 0.7f;
        }
        // Player clapping logic
        else if (player.GetComponent<PlayerMovement>().isClapping)
        {
            fadeDuration = player.GetComponent<PlayerMovement>().clapPower;
            trailRenderer.time = fadeDuration * 0.5f;
        }
        originalColor = trailRenderer.startColor;
    }

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
            var fadingColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            trailRenderer.startColor = fadingColor;
            soundWaveRenderer.material.color = fadingColor;
        }
        else
        {
            SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
            currentTime = 0;
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

}