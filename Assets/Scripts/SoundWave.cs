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
    private float trailStartTime;
    private float fadeDuration;
    private Color originalColor;
    private GameObject player;
    private bool isSneaking;
    private bool isClapping;
    private ContactFilter2D wallFilter;
    private readonly Color normalStartColor = new Color(1f, 1f, 1f, 1f);
    private readonly Color normalEndColor = new Color(1f, 1f, 1f, 0f);
    private readonly Color sneakingStartColor = new Color(1f, 1f, 1f, 0.5f);
    private readonly Color dyingStartColor = new Color(1f, 0f, 0f, 1f);
    private readonly Color dyingEndColor = new Color(1f, 0f, 0f, 0f);
    
    
    private void Awake()
    {
        reflectDir = Vector3.zero;
        rigid = GetComponent<Rigidbody2D>();
        hitInfo = new RaycastHit2D[1];
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
        rigid.Cast(moveDir, wallFilter, hitInfo);
        if (hitInfo[0].collider != null)
            reflectDir = Vector2.Reflect(moveDir, hitInfo[0].normal);

        // Fading sound wave and destroy it
        if (trailStartTime < fadeDuration)
        {
            var t = trailStartTime / fadeDuration;
            trailRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));;
        } 
        else if(fadeDuration == 0)
        {
            // Do nothing
        }
        else 
            SoundWaveGenerator.instance.RemoveSoundWave(gameObject);
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
        reflectDir = reflectDir.normalized;
        moveDir = reflectDir;
        hitInfo = new RaycastHit2D[1];
    }

    public void SetMoveDir(Vector3 dir)
    {
        moveDir = dir.normalized;
    }

    public void SetType(SoundWaveGenerator.WaveType type)
    {
        trailRenderer.startColor = normalStartColor;
        trailRenderer.endColor = normalEndColor;
        
        switch ((int) type)
        {
            case 0: // Normal
                moveSpeed = 8f;
                fadeDuration = 1.5f;
                trailRenderer.time = 1f;
                break;
            case 1: // Sneaking
                trailRenderer.startColor = sneakingStartColor;
                moveSpeed = 6f;
                trailRenderer.time = 0.55f;
                fadeDuration = 0.6f;
                break;
            case 2: // Clapping
                fadeDuration = player.GetComponentInChildren<Clap>().clapPower;
                trailRenderer.time = fadeDuration * 0.7f;
                break;
            case 3: // Diving
                fadeDuration = 2.5f;
                trailRenderer.time = 2f;
                break;
            case 4: // Death
                fadeDuration = 10f;
                trailRenderer.time = 1.5f;
                trailRenderer.startColor = dyingStartColor;
                trailRenderer.endColor = dyingEndColor;
                break;
        }
    }
}