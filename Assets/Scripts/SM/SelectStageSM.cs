using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStageSM : Singleton
{
    public GameObject stagePrefab;

    List<StageElt> stageEltList = new List<StageElt>();

    private void Start()
    {
        Set();
    }

    public void Set()
    {
        for(int i = 1; i <= gm.stageNum; i++)
        {
            if(stageEltList.Count > i)
            {
                stageEltList[i].Set(i);
                stageEltList[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject temp = Instantiate(stagePrefab, stagePrefab.transform.parent);
                StageElt elt = temp.GetComponent<StageElt>();
                elt.Set(i);
                stageEltList.Add(elt);
                temp.SetActive(true);
            }
            
        }

        for (int i = gm.stageNum; i < stageEltList.Count; i++)
            stageEltList[i].gameObject.SetActive(false);
    }
}
