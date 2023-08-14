using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveGenerator : MonoBehaviour
{
    [SerializeField] private GameObject soundWavePrefab;
    private Queue<GameObject> objectPool = new();
    public static SoundWaveGenerator instance = null;
    private int soundWaveCount;
    public enum WaveType
    {
        Normal = 0,
        Sneaking = 1,
        Clapping = 2,
        Wading = 3,
        Dying = 4
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            for (var i = 0; i < 600; i++) objectPool.Enqueue(CreateNewSoundWave());
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private GameObject CreateNewSoundWave()
    {
        var soundWave = Instantiate(soundWavePrefab, this.transform);
        soundWave.gameObject.SetActive(false);
        return soundWave;
    }

    public void SpawnSoundWave(WaveType type, Vector3 position)
    {
        switch (type)
        {
            case WaveType.Normal:
                soundWaveCount = 20;
                break;
            case WaveType.Sneaking: 
                soundWaveCount = 15;
                break;
            case WaveType.Clapping:
            case WaveType.Dying:    
                soundWaveCount = 80;
                break;
            case WaveType.Wading:
                soundWaveCount = 30;
                break;
        }

        if (objectPool.Count < soundWaveCount)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        for (var i = 1; i <= soundWaveCount; i++)
        {
            var soundWave = objectPool.Dequeue();
            soundWave.transform.position = position;
            var soundWaveScript = soundWave.GetComponent<SoundWave>();

            var angle = (360 / (float)soundWaveCount * i + 5) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            soundWaveScript.SetMoveDir(direction);
            
            soundWaveScript.SetType(type);
            soundWave.transform.SetParent(null);
            soundWave.SetActive(true);
        }
    }
    

    public void RemoveSoundWave(GameObject soundWave)
    {
        soundWave.transform.SetParent(instance.transform);
        soundWave.SetActive(false);
        instance.objectPool.Enqueue(soundWave);
    }
}