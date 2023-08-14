using System;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    public float fadeDuration; // Duration of the fade in seconds
    public float visibleDuration; // Duration of staying partially transparent in seconds

    private Renderer Footprintrenderer;
    private Color originalColor;
    private Color targetColor;
    private float fadeStartTime;
    private bool isFading;
    private bool isStop;
    private bool isSneaking;
    private GameObject player;
    private AudioSource audioSrc;
    [SerializeField] private AudioClip[] footstepSounds;

    private void Awake()
    {
        Footprintrenderer = GetComponent<Renderer>();
        player = GameObject.Find("Player");
        isFading = false;
        originalColor = Footprintrenderer.material.color;
        fadeStartTime = Time.time;
        fadeDuration = 2f;
        visibleDuration = 10f;
        audioSrc = GetComponent<AudioSource>();
        isStop = player.GetComponent<PlayerMovement>().isStop;
        
        isSneaking = player.GetComponent<PlayerMovement>().isSneaking;
        // Player sneaking logic
        if (isSneaking)
        {
            audioSrc.mute = true;
            targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            Footprintrenderer.material.color = targetColor;
        }
        else if (isStop) 
            audioSrc.mute = true;
        
        var currentFloor = (int) player.GetComponent<PlayerMovement>().currentFloor;
        // Select sound depending on current floor
        switch (currentFloor)
        {
            case 0:
                audioSrc.clip = footstepSounds[0];
                break;
            case 1:
                audioSrc.clip = footstepSounds[1];
                audioSrc.volume = 0.6f;
                break;
            default:
                Debug.Log("Error: Footprint.cs: Awake(): currentFloor is not 0 or 1");
                break;
        }
        
        audioSrc.Play();
    }


    private void Update()
    {
        // If Player is stopped, do not fade
        if (!isStop)
        {
            // Calculate the elapsed time since the fade started
            var elapsed = Time.time - fadeStartTime;

            if (!isFading)
            {
                // Fade to 50% transparency over the fade duration
                if (elapsed < fadeDuration && !isSneaking)
                {
                    var t = elapsed / fadeDuration;
                    targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0.5f, t));
                    Footprintrenderer.material.color = targetColor;
                }
                else if (elapsed < fadeDuration + visibleDuration)
                {
                    // Stay at 50% transparency for the duration of visibleDuration
                }
                else if (elapsed < fadeDuration + visibleDuration + fadeDuration)
                {
                    // Fade to fully transparent over the fade duration
                    isFading = true;
                    fadeStartTime = Time.time;
                }
                // Can't reach here
            }
            else
            {
                // Fade to fully transparent over the fade duration
                if (elapsed < fadeDuration)
                {
                    var t = elapsed / fadeDuration;
                    targetColor = new Color(originalColor.r, originalColor.g, originalColor.b,
                        Mathf.Lerp(0.5f, 0f, t));
                    Footprintrenderer.material.color = targetColor;
                }
                else
                {
                    // Object has completed the fade and should be destroyed
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            // When player move again, start fading
            if (player.GetComponent<PlayerMovement>().isStop == false)
            {
                isStop = false;
                fadeStartTime = Time.time;
            }
            // Can't reach here
        }
    }
}