using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertDevice : MonoBehaviour
{
    public SoundWaveGenerator soundWaveGenerator;
    bool deviceOn = false;
    float time = 0;
    void Update()
    {
        if(deviceOn && time > 1)
        {
            soundWaveGenerator.SpawnSoundWave(this.transform.position, false);
            time = 0;
        }
        time += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            deviceOn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            deviceOn = false;
    }
}
