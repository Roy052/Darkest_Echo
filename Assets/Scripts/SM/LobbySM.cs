using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySM : Singleton
{
    public GameObject btnPlay;
    public GameObject btnStage;
    public GameObject line;
    public GameObject btnSurvival;

    public Text labelPlay;
    public Text labelStage;
    public Text labelSurvival;
    private void Awake()
    {
        if (Singleton.lobbySM)
            Destroy(this);

        Set();
        lobbySM = this;
    }

    private void OnDestroy()
    {
        lobbySM = null;
    }
    public void Set()
    {
        StageSM.startInStage = false;
        StartCoroutine(FadeManager.FadeIn(labelPlay, 1));
        line.SetActive(false);
        btnStage.SetActive(false);
        btnSurvival.SetActive(false);
        StartCoroutine(_Set());
    }

    public void OnPlay()
    {
        StartCoroutine(_OnPlay());
    }

    public IEnumerator _OnPlay()
    {
        StartCoroutine(FadeManager.FadeOut(labelPlay, 1));

        StartCoroutine(_OnStage());

        yield return null;
        //yield return new WaitForSeconds(1);
        //btnPlay.SetActive(false);
        //btnStage.SetActive(true);
        //line.SetActive(true);
        //btnSurvival.SetActive(true);
        //StartCoroutine(FadeManager.FadeIn(labelStage, 1));
        //StartCoroutine(FadeManager.FadeIn(line.GetComponent<Image>(), 1));
        //StartCoroutine(FadeManager.FadeIn(labelSurvival, 1));
    }

    public void OnStage()
    {
        DisableBtns();
        StartCoroutine(_OnStage());
    }

    public void OnSurvival()
    {
        DisableBtns();
        //
    }

    public Transform pointsParent;
    List<Vector2> points = new List<Vector2>();
    IEnumerator _Set()
    {
        var pointCount = pointsParent.childCount;
        for(int i = 0; i < pointCount; i++)
            points.Add(pointsParent.GetChild(i).position);
        yield return new WaitForSeconds(0.5f);  
        foreach(Vector2 point in points)
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Eternal, point);
    }

    void DisableBtns()
    {
        StartCoroutine(FadeManager.FadeOut(labelStage, 1));
        StartCoroutine(FadeManager.FadeOut(line.GetComponent<Image>(), 1));
        StartCoroutine(FadeManager.FadeOut(labelSurvival, 1));
    }

    IEnumerator _OnStage()
    {
        yield return new WaitForSeconds(1);
        Singleton.gm.LoadSelectStage();
    }
}
