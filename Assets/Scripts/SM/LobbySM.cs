using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySM : MonoBehaviour
{
    public GameObject btnPlay;
    public GameObject btnStage;
    public GameObject btnSurvival;
    private void Awake()
    {
        if (Singleton.lobbySM)
            Destroy(this);

        Set();
        Singleton.lobbySM = this;
    }

    private void OnDestroy()
    {
        Singleton.lobbySM = null;
    }
    public void Set()
    {
        btnPlay.SetActive(true);
        btnStage.SetActive(false);
        btnSurvival.SetActive(false);
    }
}
