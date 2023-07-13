using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Lobby = 0,
    SelectStage = 1,
    SelectSurvival = 2,
    Stage = 3,
    Survival = 4,
}

public class GameManager : Singleton
{
    public GameState currentState = GameState.Lobby;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (gm)
            Destroy(this.gameObject);
        else
        {
            gm = this;
        }
            
    }

    private void OnDestroy()
    {
        if(gm == this)
            gm = null;
    }

    public int unlockedStageNum = 10;
    public int stageNum = 1;

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
        currentState = GameState.Lobby;
    }
    
    public void LoadSelectStage()
    {
        SceneManager.LoadScene("SelectStage");
        currentState = GameState.SelectStage;
    }

    public void LoadStage(int num)
    {
        SceneManager.LoadScene($"Stage{num}");
    }
}
