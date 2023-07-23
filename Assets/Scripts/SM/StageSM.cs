using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSM : Singleton
{
    public Text textNumber;
    public Text textTitle;
    public Image endImage;
    int stageNum;

    public GameObject player;
    public GameObject endZone;

    public GameObject wallPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] objectPrefab;

    public Transform wallParent;
    public Transform enemysParent;
    public Transform objectsParent;

    private void Awake()
    {
        stageSM = this;
    }

    private void OnDestroy()
    {
        stageSM = null;
    }

    private void Start()
    {
        textNumber.color = new Color(1, 1, 1, 0);
        textTitle.color = new Color(1, 1, 1, 0);
        player.SetActive(false);

        SetUp(gm.stageNum);
    }

    public void SetUp(int num)
    {
        stageNum = num;
        textNumber.text = $"- {Extended.ConvertToRoman(num)} -";
        textTitle.text = gameInfos.stageTitle[num];

        StartCoroutine(_SetUp());
    }

    public IEnumerator _SetUp()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeManager.FadeIn(textNumber, 1));
        StartCoroutine(FadeManager.FadeIn(textTitle, 1));
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeManager.FadeOut(textNumber, 1));
        StartCoroutine(FadeManager.FadeOut(textTitle, 1));
        yield return new WaitForSeconds(1);

        player.SetActive(true);
    }

    public void StageEnd()
    {
        StartCoroutine(_StageEnd());
    }

    public IEnumerator _StageEnd()
    {
        StartCoroutine(FadeManager.FadeIn(endImage, 1));
        yield return new WaitForSeconds(1);
        gm.stageNum += 1;
        gm.LoadStage(stageNum + 1);
    }

    public void LoadStageData()
    {
        StageData data = gm.LoadStageData(stageNum);

        
    }
}
