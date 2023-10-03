using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacles : StageObject
{
    public Color color;
    public UnityAction funcEnter;
    public UnityAction funcExit;

    AudioSource audioSrc;
    
    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Str.TagSoundWave))
        {
            other.GetComponent<SoundWave>().ChangeColor(color);
        }
        else if (other.gameObject.CompareTag(Str.TagPlayer))
        {
            audioSrc.Play();
            funcEnter?.Invoke();
        }  
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(Str.TagSoundWave))
        {
            other.GetComponent<SoundWave>().ChangeColor(Color.white);
        }
        else if (other.gameObject.CompareTag(Str.TagPlayer))
        {
            audioSrc.Play();
            funcExit?.Invoke();
        }
    }
}
