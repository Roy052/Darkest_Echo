using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveGenerator : MonoBehaviour
{
    public GameObject soundWavePrefab;
    private int soundWaveCount = 20;
    public void SpawnSoundWave(Vector3 position, bool isSneaking)
    {
        for (var i = 1; i <= soundWaveCount; i++)
        {
            var soundWave = Instantiate(soundWavePrefab);
            soundWave.transform.position = position;
            float angle = (360 / soundWaveCount * i + 5) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            soundWave.GetComponent<SoundWave>().SetMoveDir(direction);

            Color sneakColor = new Color(1f, 1f, 1f, 0.5f);
            if (isSneaking)
            {
                soundWave.GetComponent<TrailRenderer>().startColor = sneakColor;
                soundWave.GetComponent<TrailRenderer>().endColor = sneakColor;
                soundWave.GetComponent<SoundWave>().fadeDuration = 0.5f;
            }
        }
    }
}
