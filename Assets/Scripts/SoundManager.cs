using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    EndZone = 0,
    Dead = 1,
    Switch = 2,
    MovingWall = 3,
    Hungry = 4,
}

public class SoundManager : Singleton
{
    public AudioSource audioSource;
    public AudioClip[] sounds;

    private void Awake()
    {
        if(soundManager != null)
            Debug.LogError("SoundManager Is Not Null");
        soundManager = this;
    }

    private void OnDestroy()
    {
        soundManager = null;
    }

    public void PlaySound(SoundEffectType type)
    {
        if (sounds.Length <= (int)type) return;
        audioSource.clip = sounds[(int)type];
        audioSource.Play();
    }
}
