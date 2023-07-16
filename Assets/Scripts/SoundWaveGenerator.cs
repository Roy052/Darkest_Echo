using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveGenerator : MonoBehaviour
{
    public GameObject soundWavePrefab;
    public void SpawnSoundWave(Vector3 position, bool isSneaking)
    {
        var soundWave = Instantiate(soundWavePrefab);
        soundWave.transform.position = position;

        Color sneakColor = new Color(1f, 1f, 1f, 0.5f);
        if (isSneaking)
        {
            soundWave.GetComponent<TrailRenderer>().startColor = sneakColor;
            soundWave.GetComponent<TrailRenderer>().endColor = sneakColor;
            soundWave.GetComponent<SoundWave>().fadeDuration = 0.5f;
        }
    }
}
