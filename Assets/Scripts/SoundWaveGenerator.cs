using System;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;

public class SoundWaveGenerator : MonoBehaviour
{
    [SerializeField] private GameObject soundWavePrefab;
    private Queue<GameObject> objectPool = new();
    public static SoundWaveGenerator instance = null;
    private int soundWaveCount;
    private float offset;

    public bool isLoading = false;

    public enum WaveType
    {
        Normal = 0,
        Sneaking = 1,
        Clapping = 2,
        Wading = 3,
        Dying = 4,
        Throwing = 5,
        Eternal = 6,
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
        soundWave.name = $"SoundWave {objectPool.Count}";
        soundWave.gameObject.SetActive(false);
        return soundWave;
    }

    public void SpawnSoundWave(WaveType type, Vector3 position, Color? color = null)
    {
        if(isLoading)
            return;

        switch (type)
        {
            case WaveType.Normal:
                soundWaveCount = 20;
                break;
            case WaveType.Sneaking:
            case WaveType.Throwing:    
                soundWaveCount = 15;
                break;
            case WaveType.Clapping:
            case WaveType.Dying:    
                soundWaveCount = 80;
                break;
            case WaveType.Wading:
                soundWaveCount = 30;
                break;
            case WaveType.Eternal:
                soundWaveCount = 10;
                break;
        }

        if (objectPool.Count < soundWaveCount)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        offset = Random.Range(0, 5);
        for (var i = 1; i <= soundWaveCount; i++)
        {
            var soundWave = objectPool.Dequeue();
            soundWave.transform.position = position;
            var soundWaveScript = soundWave.GetComponent<SoundWave>();
            soundWaveScript.originPos = position;

            var angle = (360 / (float)soundWaveCount * i + offset) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            soundWaveScript.SetMoveDir(direction);

            soundWaveScript.SetType(type == WaveType.Throwing ? WaveType.Normal : type);
            soundWave.transform.SetParent(null);
            soundWave.SetActive(true);

            soundWaveScript.SetCreateTime(Time.timeSinceLevelLoad);

            if(color != null)
                soundWaveScript.ChangeColor(color ?? Color.white);
        }
    }
    
    public void ThrowSoundWave(Vector3 direction, Vector3 position)
    {
        if (objectPool.Count < 1)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        var soundWave = objectPool.Dequeue();
        soundWave.transform.position = position;
        var soundWaveScript = soundWave.GetComponent<SoundWave>();
        soundWaveScript.SetMoveDir(direction);
        soundWaveScript.SetType(WaveType.Throwing);
        soundWave.transform.SetParent(null);
        soundWave.SetActive(true);
    }

    public void RemoveSoundWave(GameObject soundWave)
    {
        soundWave.transform.SetParent(instance.transform);
        soundWave.SetActive(false);
        soundWave.GetComponent<SoundWave>().ClearSoundWave();
        instance.objectPool.Enqueue(soundWave);
    }

    public void ClearAllSoundWave()
    {
        while (objectPool.Count > 0)
            objectPool.Dequeue();

        var soundWaves = FindObjectsOfType<SoundWave>();
        foreach (SoundWave sw in soundWaves)
        {
            if(sw.isTemp == false)
                RemoveSoundWave(sw.gameObject);
            else
                DestroyImmediate(sw.gameObject);
        }
    }
}