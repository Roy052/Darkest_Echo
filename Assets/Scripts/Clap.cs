using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clap : MonoBehaviour
{
    private AudioSource audioSrc;
    public float clapPower;
    public bool isClapping;
    GameObject player;
    public bool isSneaking;
    
    void Start()
    {
        isClapping = false;
        audioSrc = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        isSneaking = player.GetComponentInChildren<PlayerMovement>().isSneaking;
    }

    void Update()
    {
        // Clap logic with space bar
        if (!isSneaking)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
            {
                clapPower += Time.deltaTime;
            }
            else if (Input.GetKeyUp(KeyCode.Space) && !isClapping)
            {
                isClapping = true;
                clapPower = Mathf.Clamp(clapPower, 0f, 1f);
                audioSrc.volume = Mathf.Lerp(0.5f, 1f, clapPower);
                clapPower = Mathf.Lerp(1f, 3f, clapPower);
                SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Clapping, transform.position);
                audioSrc.Play();
                clapPower = 0;
                StartCoroutine(ClapDelay());
            }
        }
    }
    
    private IEnumerator ClapDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isClapping = false;
    }
}
