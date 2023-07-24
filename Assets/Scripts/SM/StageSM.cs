using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSM : Singleton
{
    public Text textNumber;
    public Text textTitle;
    public Image endImage;
    int stageNum = 1;

    public GameObject player;
    public GameObject endZone;

    public GameObject wallPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] objectPrefab;

    public Transform wallsParent;
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
        StartCoroutine(LoadStageData());
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
        SetUp(gm.stageNum);
    }

    IEnumerator LoadStageData()
    {
        StageData data = gm.LoadStageData(stageNum);

        //Player
        Transform trPlayer = player.transform;
        trPlayer.position = data.player.position;
        trPlayer.eulerAngles = data.player.rotation;
        trPlayer.localScale = data.player.scale;

        //EndZone
        Transform trEndZone = endZone.transform;
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
            }
        }
        yield return null;
    }
}
