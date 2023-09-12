using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacles : StageObject
{
    public UnityAction funcEnter;
    public UnityAction funcExit;

    AudioSource audioSrc;
    
    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSrc.Play();
            funcEnter?.Invoke();
        }
            
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSrc.Play();
            funcExit?.Invoke();
        }
    }
}
