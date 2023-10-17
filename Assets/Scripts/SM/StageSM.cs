using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StageSM : Singleton
{
    const string StrStageFunc = "StageFuncSetup";

    int stageNum = 1;

    public Text textNumber;
    public Text textTitle;
    public Image imageEnd;

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

#if UNITY_EDITOR
        SetUp(StageSMInspector.currentStageIdx);
#else
        SetUp(gm.stageNum);
#endif
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
        imageEnd.color = Color.black;
        imageEnd.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeManager.FadeIn(textNumber, 1));
        StartCoroutine(FadeManager.FadeIn(textTitle, 1));
        yield return new WaitForSeconds(3);
        StartCoroutine(FadeManager.FadeOut(textNumber, 1));
        StartCoroutine(FadeManager.FadeOut(textTitle, 1));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeManager.FadeOut(imageEnd, 1));
        yield return new WaitForSeconds(1);

        objPlayer.SetActive(true);
    }

    public void StageEnd()
    {
        StartCoroutine(_StageEnd());
    }

    public IEnumerator _StageEnd()
    {
        imageEnd.color = Color.white;
        StartCoroutine(FadeManager.FadeIn(imageEnd, 1));
        yield return new WaitForSeconds(1);
        gm.stageNum += 1;
        StageSMInspector.currentStageIdx += 1;
        PlayerPrefs.SetInt("UnlockedStage", gm.stageNum);
        SetUp(gm.stageNum);
    }

    public void StageRestart()
    {
        StartCoroutine(_StageRestart());
    }

    public IEnumerator _StageRestart()
    {
        imageEnd.color = Color.red;
        StartCoroutine(FadeManager.FadeIn(imageEnd, 1));
        yield return new WaitForSeconds(1);
        SetUp(gm.stageNum);
    }

    IEnumerator LoadStageData()
    {
        StageData data = gm.LoadStageData(stageNum);

        objPlayer.SetActive(false);

        //Player
        Transform trPlayer = objPlayer.transform;
        trPlayer.position = data.player.position;
        trPlayer.eulerAngles = data.player.rotation;
        trPlayer.localScale = data.player.scale;

        player.canThrow = PlayerPrefs.GetInt("UnlockedStage") >= 4;

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

        while (wallsParent.childCount > data.walls.Count)
        {
            DestroyImmediate(wallsParent.GetChild(wallsParent.childCount - 1).gameObject);
        }

        //Enemy
        while (enemysParent.childCount > 1)
        {
            DestroyImmediate(enemysParent.GetChild(0).gameObject);
        }

        int scoutCount = 0;
        for (int i = 0; i < data.enemys.Count; i++)
        {
            GameObject objEnemy = Instantiate(enemyPrefab[data.enemyTypes[i]], enemysParent);
            objEnemy.name = $"Enemy {i}";
            objEnemy.SetActive(true);

            Transform trEnemy = objEnemy.transform;
            trEnemy.position = data.enemys[i].position;
            trEnemy.eulerAngles = data.enemys[i].rotation;
            trEnemy.localScale = data.enemys[i].scale;

            EnemyAI enemyAI = enemysParent.GetChild(i).GetComponent<EnemyAI>();
            enemyAI.SetEnemy();
            if(enemyAI.enemyType == EnemyType.Scout)
            {
                enemyAI.pointA = new Vector2(data.scoutPoses[scoutCount].posAX, data.scoutPoses[scoutCount].posAY);
                enemyAI.pointB = new Vector2(data.scoutPoses[scoutCount].posBX, data.scoutPoses[scoutCount].posBY);
                scoutCount++;
            }
        }

        //Object
        while (objectsParent.childCount > 1)
        {
            DestroyImmediate(objectsParent.GetChild(0).gameObject);
        }

        int areaCount = 0;
        for (int i = 0; i < data.objects.Count; i++)
        {
            GameObject objObject = Instantiate(objectPrefab[data.objectTypes[i]], objectsParent);
            objObject.SetActive(true);

            Transform trObject = objObject.transform;
            trObject.position = data.objects[i].position;
            trObject.eulerAngles = data.objects[i].rotation;
            trObject.localScale = data.objects[i].scale;
            trObject.SetAsFirstSibling();

            if (data.objectTypes[i] == (int)StageObjectType.StageArea)
            {
                StageArea stageArea;
                if (objObject.TryGetComponent(out stageArea))
                {
                    stageArea.areaNum = areaCount;
                    areaCount++;
                }
            }
            else if (data.objectTypes[i] == (int)StageObjectType.Trap)
            {
                Obstacles obstacles;
                if (objObject.TryGetComponent(out obstacles))
                {
                    obstacles.color = Color.red;
                    obstacles.funcEnterPlayer = (other) => { StageRestart(); };
                }
            }
            else if(data.objectTypes[i] == (int)StageObjectType.Water)
            {
                Obstacles obstacles;
                if (objObject.TryGetComponent(out obstacles))
                {
                    obstacles.color = Color.blue;
                }
            }
        }
        yield return null;

        Camera.main.transform.position = new Vector3(trPlayer.transform.position.x, trPlayer.transform.position.y, -10);
    }

    //Area Func
    void StageFuncSetup1()
    {
        areaFunc.Clear();
        areaFunc.Add(TutorialMove);
        areaFunc.Add(TutorialClap);
    }

    void StageFuncSetup2()
    {
        areaFunc.Clear();
        areaFunc.Add(MoveFugitiveZone21);
        areaFunc.Add(MoveFugitiveZone22);
        areaFunc.Add(MoveFugitiveZone23);
        areaFunc.Add((isEnter) => { TutorialSneak(isEnter); MoveFugitiveZone24(isEnter); });
        areaFunc.Add(MoveFugitiveZone25);
    }

    void StageFuncSetup4()
    {
        areaFunc.Clear();
        areaFunc.Add(TutorialThrow);
    }

    public Image imgTutorialMove;
    public Image imgTutorialClap;
    public Image imgTutorialSneak;
    public Image imgTutorialThrow;
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

    void TutorialThrow(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialThrow.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialThrow, 1));
        }
        else
            imgTutorialThrow.gameObject.SetActive(false);
    }

    EnemyAI enemyAi;
    void MoveFugitiveZone21(bool isEnter)
    {
        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.targetPos = new Vector2(-13.5f,2.4f);
        enemyAi.isFinding = true;
    }

    void MoveFugitiveZone22(bool isEnter)
    {
        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.targetPos = new Vector2(3.55f, -7.2f);
    }

    void MoveFugitiveZone23(bool isEnter)
    {
        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.targetPos = new Vector2(8.2f, 1.86f);
    }

    void MoveFugitiveZone24(bool isEnter)
    {
        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.targetPos = new Vector2(37.91f, 9.67f);
        enemyAi.isSneak = true;
    }

    void MoveFugitiveZone25(bool isEnter)
    {
        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.isSneak = false;
        StartCoroutine(WaitForMove());
    }

    IEnumerator WaitForMove()
    {
        yield return new WaitForSeconds(1);
        enemyAi.targetPos = new Vector2(60.17f, 9.67f);
    }
}
