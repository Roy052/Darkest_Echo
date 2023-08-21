using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StageSM : Singleton
{
    const string StrStageFunc = "StageFuncSetup";

    public Text textNumber;
    public Text textTitle;
    public Image endImage;
    int stageNum = 1;

    public GameObject objPlayer;
    public GameObject objEndZone;

    public GameObject wallPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] objectPrefab;

    public Transform wallsParent;
    public Transform enemysParent;
    public Transform objectsParent;

    //Area Func
    public List<UnityAction<bool>> areaFunc = new List<UnityAction<bool>>();

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
        objPlayer.SetActive(false);

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
        StartCoroutine(LoadStageData());
        Invoke(StrStageFunc + stageNum, 0);
        endImage.color = Color.black;
        endImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeManager.FadeIn(textNumber, 1));
        StartCoroutine(FadeManager.FadeIn(textTitle, 1));
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeManager.FadeOut(textNumber, 1));
        StartCoroutine(FadeManager.FadeOut(textTitle, 1));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeManager.FadeOut(endImage, 1));
        yield return new WaitForSeconds(1);

        objPlayer.SetActive(true);
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
        PlayerPrefs.SetInt("UnlockedStage", gm.stageNum);
        SetUp(gm.stageNum);
    }

    public void StageRestart()
    {
        StartCoroutine(_StageRestart());
    }

    public IEnumerator _StageRestart()
    {
        StartCoroutine(FadeManager.FadeIn(endImage, 1));
        yield return new WaitForSeconds(1);
        SetUp(gm.stageNum);
    }

    IEnumerator LoadStageData()
    {
        StageData data = gm.LoadStageData(stageNum);

        //Player
        Transform trPlayer = objPlayer.transform;
        trPlayer.position = data.player.position;
        trPlayer.eulerAngles = data.player.rotation;
        trPlayer.localScale = data.player.scale;

        //EndZone
        Transform trEndZone = objEndZone.transform;
        trEndZone.position = data.endZone.position;
        trEndZone.eulerAngles = data.endZone.rotation;
        trEndZone.localScale = data.endZone.scale;

        //Wall
        int count = wallsParent.childCount;
        for (int i = 0; i < data.walls.Count; i++)
        {
            if (i > count - 1)
            {
                GameObject objWall = Instantiate(wallPrefab, wallsParent);
                objWall.name = $"Wall {i}";
                objWall.SetActive(true);

                Transform trWall = objWall.transform;
                trWall.position = data.walls[i].position;
                trWall.eulerAngles = data.walls[i].rotation;
                trWall.localScale = data.walls[i].scale;
            }
            else
            {
                Transform trWall = wallsParent.GetChild(i);
                trWall.position = data.walls[i].position;
                trWall.eulerAngles = data.walls[i].rotation;
                trWall.localScale = data.walls[i].scale;
            }
        }

        //Enemy
        for (int i = 0; i < data.enemys.Count; i++)
        {
            if (i > count - 1)
            {
                GameObject objEnemy = Instantiate(enemyPrefab[data.enemyTypes[i]], enemysParent);
                objEnemy.name = $"Enemy {i}";
                objEnemy.SetActive(true);

                Transform trEnemy = objEnemy.transform;
                trEnemy.position = data.enemys[i].position;
                trEnemy.eulerAngles = data.enemys[i].rotation;
                trEnemy.localScale = data.enemys[i].scale;
            }
        }

        //Object
        int areaCount = 0;
        for (int i = 0; i < data.objects.Count; i++)
        {
            if (i > count - 1)
            {
                GameObject objObject = Instantiate(objectPrefab[data.objectTypes[i]], objectsParent);
                objObject.SetActive(true);

                Transform trObject = objObject.transform;
                trObject.position = data.objects[i].position;
                trObject.eulerAngles = data.objects[i].rotation;
                trObject.localScale = data.objects[i].scale;
                trObject.SetAsFirstSibling();

                StageArea stageArea;
                if(objObject.TryGetComponent(out stageArea))
                {
                    stageArea.areaNum = areaCount;
                    areaCount++;
                }
            }
        }
        yield return null;
    }

    
    void StageFuncSetup1()
    {
        areaFunc.Clear();
        areaFunc.Add(TutorialMove);
        areaFunc.Add(TutorialClap);
        areaFunc.Add(TutorialSneak);
    }

    public Image imgTutorialMove;
    public Image imgTutorialClap;
    public Image imgTutorialSneak;
    void TutorialMove(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialMove.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialMove, 1));
        }
        else
            imgTutorialMove.gameObject.SetActive(false);
    }

    void TutorialClap(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialClap.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialClap, 1));
        }
        else
            imgTutorialClap.gameObject.SetActive(false);
    }

    void TutorialSneak(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialSneak.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialSneak, 1));
        }
        else
            imgTutorialSneak.gameObject.SetActive(false);
    }
}
