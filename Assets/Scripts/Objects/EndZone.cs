using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name == "StageGenerator")
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
            else
#endif
                Singleton.stageSM.StageEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name == "StageGenerator")
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
            else
#endif
                Singleton.stageSM.StageEnd();
        }
        else if (collision.tag == "SoundWave")
        {
            collision.transform.GetComponent<TrailRenderer>().startWidth = 0.2f;
        }
    }
}
