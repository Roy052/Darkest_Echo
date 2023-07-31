using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveGenerator : MonoBehaviour
{
    /*
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
    */


    [SerializeField] private GameObject soundWavePrefab;
    public Queue<GameObject> objectPool = new();
    public static SoundWaveGenerator instance = null;
    private int soundWaveCount;
    private Color sneakColor = new Color(1f, 1f, 1f, 0.5f);


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            for (var i = 0; i < 400; i++) objectPool.Enqueue(CreateNewSoundWave());
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

    public void SpawnSoundWave(bool isSneaking, bool isClap, Vector3 position)
    {
        if (objectPool.Count == 0)
            for (var i = 0; i < 100; i++)
                objectPool.Enqueue(CreateNewSoundWave());

        if (isSneaking) soundWaveCount = 10;
        else if (isClap) soundWaveCount = 80;
        else soundWaveCount = 20;

        for (var i = 1; i <= soundWaveCount; i++)
        {
            var soundWave = objectPool.Dequeue();
            soundWave.transform.position = position;

            var angle = (360 / (float)soundWaveCount * i + 5) * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            soundWave.GetComponent<SoundWave>().SetMoveDir(direction);

            if (isSneaking)
            {
                soundWave.GetComponent<TrailRenderer>().startColor = sneakColor;
                soundWave.GetComponent<TrailRenderer>().endColor = sneakColor;
                soundWave.GetComponent<SoundWave>().fadeDuration = 0.5f;
            }
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