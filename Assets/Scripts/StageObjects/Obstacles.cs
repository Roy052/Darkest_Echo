using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacles : StageObject
{
    public Color color;
    
    private void Awake()
    {
        funcEnterSoundWave = (other) => { other.GetComponent<SoundWave>().ChangeColor(color); };
        funcExitSoundWave = (other) => { other.GetComponent<SoundWave>().ChangeColor(Color.white); };
    }
}
