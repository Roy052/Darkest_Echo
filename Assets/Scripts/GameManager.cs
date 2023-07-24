using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
        SceneManager.LoadScene("Stage");
    }


    public StageData LoadStageData(int stageNum)
    {
        string path = Application.dataPath + "/StageDatas";
        Debug.Log(path + "/" + StageGenerator.fileName + stageNum);   
        string data = File.ReadAllText(path + "/" + StageGenerator.fileName + stageNum);
        Debug.Log(data);
        StageData stageData = JsonUtility.FromJson<StageData>(data);

        return stageData;
    }
}
