using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;
using System;

public class StageSM : Singleton
{
#if UNITY_EDITOR
    public static bool startInStage = true;
#endif

    const string StrStageFunc = "StageFuncSetup";
    WaitForSeconds WaitForOneSecond = new WaitForSeconds(1);

    int stageNum = 1;

    [NonSerialized] public bool isPaused = false;

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

    [Header("Escape Menu")]
    public GameObject objEscapeMenu;
    public GameObject objEndBox;
    public Text textNumberEscapeMenu;
    public Text textTitleEscapeMenu;

    //Area Func
    public List<UnityAction<bool>> areaFunc = new List<UnityAction<bool>>();

    //Moving Objects
    List<MovingObject> movingObjects = new List<MovingObject>();

    //Status
    bool isTemporaryHungry = false;

    //
    bool isEnding = false;

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
        if (startInStage)
        {
            SetUp(StageSMInspector.currentStageIdx + 1);
            gm.stageNum = StageSMInspector.currentStageIdx + 1;
        }
        else
            SetUp(gm.stageNum);
#else
        SetUp(gm.stageNum);
#endif
    }

    public void SetUp(int num)
    {
        if(num > MaxStage)
        {
            gm.LoadStageEnd();
            return;
        }

        stageNum = num;
        textNumber.text = $"- {Extended.ConvertToRoman(num)} -";
        textTitle.text = gameInfos.stageTitle[num];
        textNumberEscapeMenu.text = $"- {Extended.ConvertToRoman(num)} -";
        textTitleEscapeMenu.text = gameInfos.stageTitle[num];

        StartCoroutine(_SetUp());
    }

    public IEnumerator _SetUp()
    {
        StartCoroutine(LoadStageData());
        areaFunc.Clear();
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
        SoundWaveGenerator.instance.isLoading = false;
    }

    public void StageEnd()
    {
        if (isEnding) return;

        StartCoroutine(_StageEnd());
        isEnding = true;
    }

    public IEnumerator _StageEnd()
    {
        soundManager.PlaySound(SoundEffectType.EndZone);
        imageEnd.color = Color.white;
        StartCoroutine(FadeManager.FadeIn(imageEnd, 1));
        yield return new WaitForSeconds(1);
        stageNum += 1;
        gm.stageNum += 1;
        gm.unlockedStageNum = Mathf.Max(gm.unlockedStageNum, gm.stageNum);
#if UNITY_EDITOR
        StageSMInspector.currentStageIdx += 1;
#endif
        ResetStatus();
        PlayerPrefs.SetInt("UnlockedStage", gm.stageNum);
        SetUp(gm.stageNum);
    }

    public void StageRestart()
    {
        if (isEnding) return;

        StartCoroutine(_StageRestart(false));
        isEnding = true;
    }

    public IEnumerator _StageRestart(bool isByMenu)
    {
        if (isByMenu == false)
        {
            soundManager.PlaySound(SoundEffectType.Dead);
            imageEnd.color = Color.red;
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Dying, player.transform.position, Color.red);
        }
        StartCoroutine(FadeManager.FadeIn(imageEnd, 1));
        yield return new WaitForSeconds(1);
        ResetStatus();
        SetUp(gm.stageNum);
    }

    public void StageRestartByMenu()
    {
        StartCoroutine(_StageRestart(true));
        isEnding = true;
    }

    public void StageExit()
    {
        ResetStatus();
    }

    void ResetStatus()
    {
        SoundWaveGenerator.instance.isLoading = true;
        isTemporaryHungry = false;
        isEnding = false;
        player.isSneaking = false;
        player.isWater = false;
        player.isHungry = false;
        SoundWaveGenerator.instance.ClearAllSoundWave();
        foreach (Footprint fp in FindObjectsOfType<Footprint>())
            Destroy(fp.gameObject);
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
        while (enemysParent.childCount > 0)
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
        movingObjects.Clear();
        while (objectsParent.childCount > 0)
        {
            DestroyImmediate(objectsParent.GetChild(0).gameObject);
        }

        int areaCount = 0;
        int movingObjectCount = 0;
        for (int i = 0; i < data.objects.Count; i++)
        {
            GameObject objObject = Instantiate(objectPrefab[data.objectTypes[i]], objectsParent);
            objObject.SetActive(true);

            Transform trObject = objObject.transform;
            trObject.position = data.objects[i].position;
            trObject.eulerAngles = data.objects[i].rotation;
            trObject.localScale = data.objects[i].scale;

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
            else if (data.objectTypes[i] == (int)StageObjectType.MovingObject)
            {
                MovingObject movingObject;
                if (objObject.TryGetComponent(out movingObject))
                {
                    movingObject.enterPos = new Vector2(data.movingObjectEnterPoses[movingObjectCount].posX, data.movingObjectEnterPoses[movingObjectCount].posY);
                    movingObject.exitPos = new Vector2(data.movingObjectExitPoses[movingObjectCount].posX, data.movingObjectExitPoses[movingObjectCount].posY);
                    movingObjectCount++;
                    movingObjects.Add(movingObject);
                }
            }
            else if (data.objectTypes[i] == (int)StageObjectType.UnlockZone)
            {
                StageArea stageArea;
                if (objObject.TryGetComponent(out stageArea))
                {
                    stageArea.areaNum = areaCount;
                    areaCount++;
                }
            }
        }
        yield return null;

        Camera.main.transform.position = new Vector3(trPlayer.transform.position.x, trPlayer.transform.position.y, -10);

        gridInstance.CreateGrid();
    }

    //Area Func
    void StageFuncSetup1()
    {
        areaFunc.Add(TutorialMove);
        areaFunc.Add(TutorialClap);
    }

    void StageFuncSetup2()
    {
        areaFunc.Add(MoveFugitiveZone21);
        areaFunc.Add(MoveFugitiveZone22);
        areaFunc.Add(MoveFugitiveZone23);
        areaFunc.Add((isEnter) => { TutorialSneak(isEnter); MoveFugitiveZone24(isEnter); });
        areaFunc.Add(MoveFugitiveZone25);
    }

    void StageFuncSetup4()
    {
        areaFunc.Add(MoveWallZone41);
        areaFunc.Add(TutorialThrow);
    }

    void StageFuncSetup5()
    {
        areaFunc.Add(TemporaryHungry);
        areaFunc.Add(Hungry);
    }

    void StageFuncSetup6()
    {
        areaFunc.Add(Hungry);
        areaFunc.Add(MoveFugitiveZone61);
        areaFunc.Add(MoveFugitiveZone62);
        areaFunc.Add(MoveFugitiveZone63);
        areaFunc.Add(MoveFugitiveZone64);
        areaFunc.Add(MoveFugitiveZone65);
        areaFunc.Add(MoveFugitiveZone66);
        areaFunc.Add(MoveFugitiveZone67);
        areaFunc.Add(MoveFugitiveZone68);
        areaFunc.Add(MoveFugitiveZone69);
        areaFunc.Add(MoveFugitiveZone69_1);
        areaFunc.Add(MoveFugitiveZone69_2);
        areaFunc.Add(MoveFugitiveZone69_3);
        areaFunc.Add(MoveFugitiveZone69_4);
        areaFunc.Add(MoveFugitiveZone69_5);
        areaFunc.Add(MoveFugitiveZone69_6);
    }

    void StageFuncSetup7()
    {
        areaFunc.Add(HungryToFull);
        areaFunc.Add(MoveWallZone71);
        areaFunc.Add(MoveWallZone72);
        areaFunc.Add(MoveWallZone73);
        areaFunc.Add(MoveWallZone74);
        areaFunc.Add(MoveWallZone75);
        areaFunc.Add(MoveWallZone76);
    }

    void StageFuncSetup8()
    {
        areaFunc.Add(MoveWallZone81);
    }
    void StageFuncSetup9()
    {

    }
    void StageFuncSetup10()
    {
        areaFunc.Add(MoveWallZone101);
        areaFunc.Add(MoveWallZone102);
        areaFunc.Add(MoveWallZone103);
    }

    public Image imgTutorialMove;
    public Image imgTutorialClap;
    public Image imgTutorialSneak;
    public Image imgTutorialThrow0;
    public Image imgTutorialThrow1;
    void TutorialMove(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialMove.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialMove, 1));
        }
        else
            StartCoroutine(FadeManager.FadeOut(imgTutorialMove, 1));

    }

    void TutorialClap(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialClap.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialClap, 1));
        }
        else
            StartCoroutine(FadeManager.FadeOut(imgTutorialClap, 1));
    }

    void TutorialSneak(bool isEnter)
    {
        if (isEnter)
        {
            imgTutorialSneak.gameObject.SetActive(true);
            StartCoroutine(FadeManager.FadeIn(imgTutorialSneak, 1));
        }
        else 
            StartCoroutine(FadeManager.FadeOut(imgTutorialSneak, 1));
    }

    Coroutine tutorialThrow;
    void TutorialThrow(bool isEnter)
    {
        if (isEnter)
        {
            tutorialThrow = StartCoroutine(_TutorialThrow());
            player.canThrow = true; 
        }
        else
        {
            if (tutorialThrow != null)
                StopCoroutine(tutorialThrow);
            imgTutorialThrow0.gameObject.SetActive(false);
            imgTutorialThrow1.gameObject.SetActive(false);
        }
    }

    IEnumerator _TutorialThrow()
    {
        RectTransform rect = imgTutorialThrow1.transform as RectTransform;
        Vector3 currentPos = rect.anchoredPosition;
        Vector3 origin = rect.anchoredPosition;
        Vector3 dest = origin + new Vector3(80, 80, 0);
        while (true)
        {
            imgTutorialThrow0.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            imgTutorialThrow0.gameObject.SetActive(false);
            imgTutorialThrow1.gameObject.SetActive(true);

            while(currentPos.x < dest.x)
            {
                currentPos += new Vector3(80, 80, 0) * Time.deltaTime;
                rect.anchoredPosition = currentPos;
                yield return null;
            }

            imgTutorialThrow1.gameObject.SetActive(false);
            currentPos = origin;
            rect.anchoredPosition = currentPos;
        }
    }

    EnemyAI enemyAi;
    void MoveFugitiveZone21(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-8.5f,2.4f));
        enemyAi.isFinding = true;
    }

    void MoveFugitiveZone22(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(3.55f, -7.2f));
    }

    void MoveFugitiveZone23(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(8.2f, 1.86f));
    }

    void MoveFugitiveZone24(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(37.91f, 9.67f));
        enemyAi.isSneak = true;
    }

    void MoveFugitiveZone25(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.isSneak = false;
        movingObjects[0].StartCoroutine(movingObjects[0].OnEnterPos());
        StartCoroutine(WaitForMove());
        StartCoroutine(PlayWallSound());
    }

    void MoveFugitiveZone61(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-67.5f, -9.7f));
    }

    void MoveFugitiveZone62(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-53f, -9.7f));
    }

    void MoveFugitiveZone63(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-53f, 4.4f));
    }

    void MoveFugitiveZone64(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-25.5f, 4.4f));
    }

    void MoveFugitiveZone65(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(-25.5f, -20.5f));
    }

    void MoveFugitiveZone66(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(8.3f, -20.5f));
    }

    void MoveFugitiveZone67(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(16.3f, -20.5f));
    }

    void MoveFugitiveZone68(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(16.3f, -2.2f));
    }

    void MoveFugitiveZone69(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(32.5f, -2.2f));
    }

    void MoveFugitiveZone69_1(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(32.5f, -20.7f));
    }

    void MoveFugitiveZone69_2(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(40.3f, -20.7f));
    }

    void MoveFugitiveZone69_3(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(40.3f, -16f));
    }

    void MoveFugitiveZone69_4(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(51.9f, -16f));
    }

    void MoveFugitiveZone69_5(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(51.9f, -2.4f));
    }

    void MoveFugitiveZone69_6(bool isEnter)
    {
        if (isEnter == false) return;

        if (enemyAi == null)
            enemyAi = enemysParent.GetChild(0).GetComponent<EnemyAI>();
        enemyAi.MoveFugitive(new Vector2(68f, -2.4f));
    }

    void MoveWallZone41(bool isEnter)
    {
        if (isEnter)
        {
            movingObjects[0].StartCoroutine(movingObjects[0].OnEnterPos());
            StartCoroutine(PlayWallSound());
        }
        else
            movingObjects[0].StartCoroutine(movingObjects[0].OnExitPos());
    }

    void MoveWallZone71(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[0].StartCoroutine(movingObjects[0].OnEnterPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone72(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[1].StartCoroutine(movingObjects[1].OnEnterPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone73(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[1].StartCoroutine(movingObjects[1].OnExitPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone74(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[2].StartCoroutine(movingObjects[2].OnEnterPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone75(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[2].StartCoroutine(movingObjects[2].OnExitPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone76(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[3].StartCoroutine(movingObjects[3].OnEnterPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone81(bool isEnter)
    {
        if (isEnter == false) return;

        movingObjects[0].StartCoroutine(movingObjects[0].OnEnterPos());
        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone101(bool isEnter)
    {
        if (isEnter)
            movingObjects[0].StartCoroutine(movingObjects[0].OnEnterPos());
        else
            movingObjects[0].StartCoroutine(movingObjects[0].OnExitPos());

        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone102(bool isEnter)
    {
        if (isEnter)
            movingObjects[1].StartCoroutine(movingObjects[1].OnEnterPos());
        else
            movingObjects[1].StartCoroutine(movingObjects[1].OnExitPos());

        StartCoroutine(PlayWallSound());
    }

    void MoveWallZone103(bool isEnter)
    {
        if (isEnter)
            movingObjects[2].StartCoroutine(movingObjects[2].OnEnterPos());
        else
            movingObjects[2].StartCoroutine(movingObjects[2].OnExitPos());


    }

    void TemporaryHungry(bool isEnter)
    {
        if (isTemporaryHungry) return;
        isTemporaryHungry = true;
        StartCoroutine(_TemporaryHungry());
    }

    IEnumerator _TemporaryHungry()
    {
        player.isHungry = true;
        yield return new WaitForSeconds(1);
        player.isHungry = false;
    }

    WaitForSeconds waitForHalfSecond = new WaitForSeconds(0.5f);
    IEnumerator PlayWallSound()
    {
        soundManager.PlaySound(SoundEffectType.Switch);
        yield return waitForHalfSecond;
        soundManager.PlaySound(SoundEffectType.MovingWall);
    }

    void Hungry(bool isEnter)
    {
        player.isHungry = true;
    }

    void HungryToFull(bool isEnter)
    {
        StartCoroutine(_HungryToFull());
    }

    IEnumerator _HungryToFull()
    {
        Color temp = Color.red;

        yield return new WaitForSeconds(1);
        while(temp.b >= 1)
        {
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal, transform.localPosition, temp);
            temp.b += 0.34f;
            temp.g += 0.34f;
            yield return new WaitForSeconds(0.34f);
        }
    }

    IEnumerator WaitForMove()
    {
        yield return new WaitForSeconds(1);
        gridInstance.RefreshGrid();
        enemyAi.MoveFugitive(new Vector2(60.17f, 9.67f));
    }

    //Escape Menu
    private void Update()
    {
        if (isEnding == false && Input.GetKeyUp(KeyCode.Escape))
        {
            if (objEndBox.activeSelf == false)
                objEscapeMenu.SetActive(objEscapeMenu.activeSelf == false);
            else
                objEndBox.SetActive(objEndBox.activeSelf == false);

            isPaused = objEscapeMenu.activeSelf;
        }
    }

    public void OnClickEscapeMenu()
    {
        objEscapeMenu.SetActive(true);
        isPaused = true;
    }

    public void OnClickResume()
    {
        objEscapeMenu.SetActive(false);
        isPaused = false;
    }
    public void OnClickRestart()
    {
        StageRestartByMenu();
        objEscapeMenu.SetActive(false);
        isPaused = false;
        isEnding = true;
    }

    public void OnClickLevelSelect()
    {
        isEnding = true;
        objEscapeMenu.SetActive(false);
        StartCoroutine(_LevelSelect());
    }

    IEnumerator _LevelSelect()
    {
        StartCoroutine(FadeManager.FadeIn(imageEnd, 1));
        yield return new WaitForSeconds(1f);
        gm.LoadSelectStage();
    }

    public void OnClickExit()
    {
        objEndBox.SetActive(true);
    }

    public void OnClickExitYes()
    {
        Application.Quit();
    }

    public void OnClickExitNo()
    {
        objEndBox.SetActive(false);
    }
}
