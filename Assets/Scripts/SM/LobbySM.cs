using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySM : MonoBehaviour
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
        Singleton.lobbySM = this;
    }

    private void OnDestroy()
    {
        Singleton.lobbySM = null;
    }
    public void Set()
    {
        StartCoroutine(FadeManager.FadeIn(labelPlay, 1));
        line.SetActive(false);
        btnStage.SetActive(false);
        btnSurvival.SetActive(false);
    }

    public void OnPlay()
    {
        StartCoroutine(_OnPlay());
    }

    public IEnumerator _OnPlay()
    {
        StartCoroutine(FadeManager.FadeOut(labelPlay, 1));

        yield return new WaitForSeconds(1);

        btnPlay.SetActive(false);
        btnStage.SetActive(true);
        line.SetActive(true);
        btnSurvival.SetActive(true);
        StartCoroutine(FadeManager.FadeIn(labelStage, 1));
        StartCoroutine(FadeManager.FadeIn(line.GetComponent<Image>(), 1));
        StartCoroutine(FadeManager.FadeIn(labelSurvival, 1));
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

    public void DisableBtns()
    {
        StartCoroutine(FadeManager.FadeOut(labelStage, 1));
        StartCoroutine(FadeManager.FadeOut(line.GetComponent<Image>(), 1));
        StartCoroutine(FadeManager.FadeOut(labelSurvival, 1));
    }

    public IEnumerator _OnStage()
    {
        yield return new WaitForSeconds(1);
        Singleton.gm.LoadSelectStage();
    }
}
