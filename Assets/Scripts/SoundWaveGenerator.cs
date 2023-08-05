using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveGenerator : MonoBehaviour
{
    [SerializeField] private GameObject soundWavePrefab;
    public Queue<GameObject> objectPool = new();
    public static SoundWaveGenerator instance = null;
    private int soundWaveCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            for (var i = 0; i < 500; i++) objectPool.Enqueue(CreateNewSoundWave());
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        soundWaveCount = 20;
    }

    private GameObject CreateNewSoundWave()
    {
        var soundWave = Instantiate(soundWavePrefab, this.transform);
        soundWave.gameObject.SetActive(false);
        return soundWave;
    }

    public void SpawnSoundWave(bool isSneaking, bool isClapping, Vector3 position)
    {

        if (isSneaking) soundWaveCount = 15;
        else if (isClapping) soundWaveCount = 80;
        else soundWaveCount = 20;

        if (objectPool.Count < soundWaveCount)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        for (var i = 1; i <= soundWaveCount; i++)
        {
            var soundWave = objectPool.Dequeue();
            soundWave.transform.position = position;

            var angle = (360 / (float)soundWaveCount * i + 5) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            soundWave.GetComponent<SoundWave>().SetMoveDir(direction);
            
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