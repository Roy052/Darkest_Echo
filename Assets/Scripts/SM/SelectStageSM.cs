using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageSM : Singleton
{
    public GameObject stagePrefab;
    public GameObject linePrefab;

    public Sprite[] spriteRomans;

    public CameraDragController cameraDragController;

    List<StageElt> stageEltList = new List<StageElt>();
    List<Line> lines = new List<Line>();

    private void Awake()
    {
        selectStageSM = this;
    }

    private void OnDestroy()
    {
        selectStageSM = null;
    }

    private void Start()
    {
        Set();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            gameEndBox.gameObject.SetActive(gameEndBox.gameObject.activeSelf == false);
    }

    public void Set()
    {
        Vector3 pos = new Vector3(0, 0);
        int unlockedNum = gm.unlockedStageNum;
        for (int i = 0; i < unlockedNum; i++)
        {
            GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
            temp.transform.position = pos;
            temp.name = $"Stage Elt {i + 1}";
            StageElt elt = temp.GetComponent<StageElt>();
            elt.Set(i + 1, false);
            stageEltList.Add(elt);
            temp.SetActive(true);
            pos.x += 5;
        }

        if (unlockedNum < MaxStage)
        {
            GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
            temp.transform.position = pos;
            temp.name = $"Stage Elt {unlockedNum + 1}";
            StageElt elt = temp.GetComponent<StageElt>();
            elt.Set(unlockedNum + 1, true);
            stageEltList.Add(elt);
            temp.SetActive(true);
        }
        
        pos.x += 5;

        cameraDragController.SetCameraPosMax(gm.unlockedStageNum * 5);

        for (int i = 0; i < unlockedNum - 1; i++)
        {
            GameObject tempLine = Instantiate(linePrefab, linePrefab.transform.parent);
            tempLine.name = $"Line {i + 1}";
            Line line = tempLine.GetComponent<Line>();
            line.Set(stageEltList[i].gameObject, stageEltList[i + 1].gameObject);
            lines.Add(line);
            tempLine.SetActive(true);
        }

        for (int i = gm.stageNum; i < stageEltList.Count; i++)
            stageEltList[i].gameObject.SetActive(false);
    }

    public void OnQuit()
    {
        gameEndBox.gameObject.SetActive(true);
    }

    public Image gameEndBox;

    public void OnQuitYes()
    {
        Application.Quit();
    }

    public void OnQuitNo()
    {
        gameEndBox.gameObject.SetActive(false);
    }
}
