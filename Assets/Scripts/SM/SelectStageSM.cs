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

    public void Set()
    {
        Vector3 pos = new Vector3(0, 0);
        for(int i = 0; i < gm.unlockedStageNum; i++)
        {
            if(stageEltList.Count > i)
            {
                stageEltList[i].Set(i + 1);
                stageEltList[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
                temp.transform.position = pos;
                temp.name = $"Stage Elt {i + 1}";
                StageElt elt = temp.GetComponent<StageElt>();
                elt.Set(i + 1);
                stageEltList.Add(elt);
                temp.SetActive(true);
            }

            pos.x += 5;
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
