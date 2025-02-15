using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageElt : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    const float MaxScale = 1f;
    const float MinScale = 0.95f;

    public GameObject objSpriteRoman;
    public GameObject objInside;
    public GameObject objLock;

    int num = 1;

    bool isLock = false;
    bool isUp = false;

    float time = 0;
    float endTime = (float)Singleton.random.NextDouble() * 2 + 1;
    float limitDown = -2, limitUp = 2;
    float speed = 0.2f;

    public void Set(int num, bool isLock)
    {
        this.isLock = isLock;
        objLock.SetActive(isLock);
        objSpriteRoman.SetActive(isLock == false);

        if (isLock == false)
        {
            objSpriteRoman.GetComponent<SpriteRenderer>().sprite = Singleton.selectStageSM.spriteRomans[num - 1];
            this.num = num;
        }
        else
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.6f);
    }

    private void Start()
    {
        isUp = num % 2 == 0;
    }

    private void Update()
    {
        transform.localPosition += new Vector3(0, speed * Time.deltaTime * (isUp ? 1 : -1));

        if (time >= endTime || transform.position.y <= limitDown || transform.position.y >= limitUp)
        {
            time = 0;
            endTime = (float)Singleton.random.NextDouble() * 2 + 1;
            isUp = Singleton.random.Next(0, 2) == 0;

            if (transform.position.y < limitDown) isUp = true;
            else if (transform.position.y > limitUp) isUp = false;
        }

        time += Time.deltaTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("A");
        if (isLock) return;
        Debug.Log("Stage : " + num);

        Singleton.gm.stageNum = num;
        Singleton.gm.LoadStage(num);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (changeScale != null)
            StopCoroutine(changeScale);
        objInside.transform.localScale = new Vector3(MinScale, MinScale, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (changeScale != null)
            StopCoroutine(changeScale);

        changeScale = StartCoroutine(ChangeInsideScale(0.8f));
    }

    Coroutine changeScale = null;
    IEnumerator ChangeInsideScale(float delay)
    {
        float time = 0f;

        while (time < delay)
        {
            float temp = Mathf.Lerp(MinScale, MaxScale, time / delay);
            objInside.transform.localScale = new Vector3(temp,temp, delay);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
