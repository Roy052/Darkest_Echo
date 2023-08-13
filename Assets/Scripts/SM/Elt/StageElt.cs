using System;
using UnityEngine;

public class StageElt : MonoBehaviour
{
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

        if(isLock == false)
        {
            objSpriteRoman.GetComponent<SpriteRenderer>().sprite = Singleton.selectStageSM.spriteRomans[num - 1];
            this.num = num;
        }
    }

    private void Start()
    {
        isUp = num % 2 == 0;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // Check for left mouse button down
        //{
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //    RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        //    if(hit.collider != null)
        //    {
        //        OnMouseDown();
        //    }
        //}

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

    private void OnMouseDown()
    {
        Debug.Log("A");
        if (isLock) return;
        Debug.Log("Stage : " + num);

        Singleton.gm.stageNum = num;
        Singleton.gm.LoadStage(num);
    }
}
