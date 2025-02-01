using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class StageEndSM : Singleton
{
    public Image imgShadow; 
    public PostProcessVolume volume;       // Inspector에서 할당
    public float shutterMin = 0f;
    public float shutterMax = 360f;
    public float duration = 3f;

    float timer = 0f;
    MotionBlur mb;

    IEnumerator MotionBlur()
    {
        if (mb == null) yield break;
        
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float currentValue = Mathf.Lerp(shutterMin, shutterMax, t);
            mb.shutterAngle.value = currentValue;
            yield return null;
        }
    }

    IEnumerator Start()
    {
        if (volume != null && volume.profile != null)
            volume.profile.TryGetSettings(out mb);
        yield return null;
        yield return StartCoroutine(FadeManager.FadeOut(imgShadow, 2));
        yield return StartCoroutine(MotionBlur());

    }

    public void GoToLobby()
    {
        gm.LoadLobby();
    }
}
