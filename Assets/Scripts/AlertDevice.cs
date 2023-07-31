using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertDevice : StageObject
{
    bool deviceOn = false;
    float time = 0;

    private void Awake()
    {
        type = StageObjectType.AlertDevice;
    }

    void Update()
    {
        if(deviceOn && time > 1)
        {
            SoundWaveGenerator.instance.SpawnSoundWave(false, false, transform.position);
            time = 0;
        }
        time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            deviceOn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            deviceOn = false;
    }
}
