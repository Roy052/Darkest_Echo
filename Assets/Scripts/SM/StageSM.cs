using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSM : Singleton
{
    public Text textNumber;
    public Text textTitle;
    int stageNum;

    private void Start()
    {
        textNumber.color = new Color(1, 1, 1, 0);
        textTitle.color = new Color(1, 1, 1, 0);
        SetUp(gm.stageNum);
    }

    public void SetUp(int num)
    {
        stageNum = num;
        textNumber.text = $"- {Extended.ConvertToRoman(num)} -";

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
    }

    public IEnumerator StageEnd()
    {
        yield return new WaitForSeconds(1);
    }
}
