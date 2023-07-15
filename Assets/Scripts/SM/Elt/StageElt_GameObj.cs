using System;
using UnityEngine;

public class StageElt_GameObj : MonoBehaviour
{
    public GameObject spriteRoman;
    public GameObject inside;

    int num = 1;

    public void Set(int num)
    {
        spriteRoman.GetComponent<SpriteRenderer>().sprite = Singleton.selectStageTestSM.spriteRomans[num - 1];
        this.num = num;
    }

    bool isUp = false;

    private void Start()
    {
        isUp = num % 2 == 0;
    }

    float time = 0;
    float endTime = (float)Singleton.random.NextDouble() * 2 + 1;
    float limitDown = -2, limitUp = 2;
    float speed = 0.05f;

    private void Update()
    {
        transform.localPosition += new Vector3(0, speed * Time.deltaTime * (isUp ? 1 : -1));
        //spriteRoman.transform.localPosition += new Vector3(0, speed * Time.deltaTime * (isUp ? 1 : -1));
        //inside.transform.localPosition += new Vector3(0, speed * Time.deltaTime * (isUp ? 1 : -1));

        if (time >= endTime)
        {
            time = 0;
            endTime = (float)Singleton.random.NextDouble() * 2 + 1;
            //Debug.Log($"{num} : {endTime}");
            isUp = endTime > 1.5f;

            if (transform.position.y < limitDown) isUp = true;
            else if (transform.position.y > limitUp) isUp = false;
        }

        time += Time.deltaTime;
    }

    private void OnMouseDown()
    {
        Singleton.gm.stageNum = num;
        Singleton.gm.LoadStage(num);
    }
}
