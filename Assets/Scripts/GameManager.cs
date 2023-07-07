using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton
{
    private void Awake()
    {
        if (gm)
            Destroy(this);
        else
            gm = this;
    }

    private void OnDestroy()
    {
        gm = null;
    }

    public int stageNum = 10;

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void LoadStage(int num)
    {
        SceneManager.LoadScene($"Stage{num}");
    }
}
