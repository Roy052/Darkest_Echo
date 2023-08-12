using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalSM : Singleton
{
    public Vector2[] movePoints;

    private void Awake()
    {
        survivalSM = this;
    }

    private void OnDestroy()
    {
        survivalSM = null;
    }

    public void SurvivalEnd()
    {

    }
}
