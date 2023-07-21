using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (SceneManager.GetActiveScene().name == "StageGenerator")
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        if (collision.collider.tag == "Player")
        {
            Singleton.stageSM.StageEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.tag == "Player")
        {
            Singleton.stageSM.StageEnd();
        }
    }
}
