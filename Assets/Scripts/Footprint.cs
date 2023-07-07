using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class Footprint : MonoBehaviour
{
    public float fadeDuration = 2f;      // Duration of the fade in seconds
    public float visibleDuration = 10f;  // Duration of staying partially transparent in seconds

    private Renderer renderer;
    private Color originalColor;
    private float fadeStartTime;
    private bool isFading = false;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
        fadeStartTime = Time.time;
    }


    void Update()
    {
        // Calculate the elapsed time since the fade started
        float elapsed = Time.time - fadeStartTime;

        if (!isFading)
        {
            // Fade to 50% transparency over the fade duration
            if (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0.5f, t));
                renderer.material.color = newColor;
            }
            else if (elapsed < (fadeDuration + visibleDuration))
            {
                // Stay partially transparent for the visible duration
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
                renderer.material.color = newColor;
            }
            else if (elapsed < (fadeDuration + visibleDuration + fadeDuration))
            {
                // Fade to fully transparent over the fade duration
                isFading = true;
                fadeStartTime = Time.time;
            }
            else
            {
                // Object has completed the fade and should be destroyed
                Destroy(gameObject);
            }
        }
        else
        {
            // Fade to fully transparent over the fade duration
            if (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.5f, 0f, t));
                renderer.material.color = newColor;
            }
            else
            {
                // Object has completed the fade and should be destroyed
                Destroy(gameObject);
            }
        }
    }
}
