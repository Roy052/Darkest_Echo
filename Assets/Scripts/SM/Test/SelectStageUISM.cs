using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageUISM : Singleton
{
    public GameObject stagePrefab;
    public GameObject linePrefab;

    public Sprite[] spriteRomans;

    public CameraDragController cameraDragController;

    List<StageEltUI> stageEltList = new List<StageEltUI>();

    private void Awake()
    {
        selectStageUISM = this;
    }

    private void OnDestroy()
    {
        selectStageUISM = null;
    }

    private void Start()
    {
        Set();
    }

    public void Set()
    {
        Vector3 pos = new Vector3(0, 0);
        int unlockedNum = gm.unlockedStageNum;
        for(int i = 0; i <= unlockedNum; i++)
        {
            GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
            temp.transform.position = pos;
            temp.name = $"Stage Elt {i + 1}";
            StageEltUI elt = temp.GetComponent<StageEltUI>();
            elt.Set(i + 1, false);
            stageEltList.Add(elt);
            temp.SetActive(true);

            pos.x += 5;
        }

        if (unlockedNum < MaxStage) {
            GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
            temp.transform.position = pos;
            temp.name = $"Stage Elt {unlockedNum + 1}";
            StageEltUI elt = temp.GetComponent<StageEltUI>();
            elt.Set(unlockedNum + 2, true);
            stageEltList.Add(elt);
            temp.SetActive(true);
        }

        cameraDragController.SetCameraPosMax(gm.unlockedStageNum * 5);

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
