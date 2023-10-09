using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertDevice : StageObject
{
    SoundWaveGenerator soundWaveGenerator;
    bool deviceOn = false;
    float time = 0;

    private void Awake()
    {
        soundWaveGenerator = SoundWaveGenerator.instance;
        funcEnterPlayer = (other) => { deviceOn = true; };
        funcExitPlayer = (other) => { deviceOn = false; };
    }

    void Update()
    {
        if(deviceOn && time > 1)
        {
            soundWaveGenerator.SpawnSoundWave(SoundWaveGenerator.WaveType.Sneaking, transform.position);
            time = 0;
        }
        time += Time.deltaTime;
    }
}
