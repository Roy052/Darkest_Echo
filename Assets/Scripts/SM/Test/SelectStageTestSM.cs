using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageTestSM : Singleton
{
    public GameObject stagePrefab;
    public GameObject linePrefab;

    public Sprite[] spriteRomans;

    public CameraDragController cameraDragController;

    List<StageElt> stageEltList = new List<StageElt>();
    List<StageElt_GameObj> stageEltGameObjList = new List<StageElt_GameObj>();
    List<Line> lines = new List<Line>();
    List<Line_GameObj> lineObjs = new List<Line_GameObj>();

    private void Awake()
    {
        selectStageTestSM = this;
    }

    private void OnDestroy()
    {
        selectStageTestSM = null;
    }

    private void Start()
    {
        Set();
    }

    public void Set()
    {
        Vector3 pos = new Vector3(0, 0);
        for (int i = 0; i < gm.unlockedStageNum; i++)
        {
            if (stageEltList.Count > i)
            {
                stageEltList[i].Set(i + 1);
                stageEltList[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
                temp.transform.position = pos;
                temp.name = $"Stage Elt {i + 1}";
                StageElt_GameObj elt = temp.GetComponent<StageElt_GameObj>();
                elt.Set(i + 1);
                stageEltGameObjList.Add(elt);
                temp.SetActive(true);
            }

            pos.x += 5;
        }

        cameraDragController.SetCameraPosMax(gm.unlockedStageNum * 5);

        for (int i = 0; i < gm.unlockedStageNum - 2; i++)
        {
            GameObject tempLine = Instantiate(linePrefab, linePrefab.transform.parent);
            tempLine.name = $"Line {i + 1}";
            Line_GameObj line = tempLine.GetComponent<Line_GameObj>();
            line.Set(stageEltGameObjList[i].gameObject, stageEltGameObjList[i + 1].gameObject);
            lineObjs.Add(line);
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
