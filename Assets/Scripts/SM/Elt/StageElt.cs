using System;
using UnityEngine;
using UnityEngine.UI;

public class StageElt : MonoBehaviour
{
    public Text textNumber;
    int num = 0;

    public void Set(int num)
    {
        textNumber.text = Extended.ConvertToRoman(num);
        this.num = num;
    }

    public void OnTouch()
    {
        Singleton.gm.stageNum = num;
        Singleton.gm.LoadStage(num);
    }

    bool isUp = false;
    RectTransform rect;

    private void Start()
    {
        isUp = num % 2 == 0;
        rect = this.gameObject.GetComponent<RectTransform>();
    }

    float time = 0;
    float endTime = (float) Singleton.random.NextDouble() * 2 + 1;
    float limitDown = -500, limitUp = -250;
    private void Update()
    {
        rect.localPosition += new Vector3(0, 5 * Time.deltaTime * (isUp ? 1 : -1));

        if(time >= endTime)
        {
            time = 0;
            endTime = (float) Singleton.random.NextDouble() * 2 + 1;
            //Debug.Log($"{num} : {endTime}");
            isUp = endTime > 1.5f;

            if (rect.localPosition.y < limitDown) isUp = true;
            else if (rect.localPosition.y > limitUp) isUp = false;
        }

        time += Time.deltaTime;
    }
}
