using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : StageObject
{
    public Vector2 enterPos;
    public Vector2 exitPos;

    Vector2 startPos;
    float time = 0;

    private void Awake()
    {
        startPos = transform.position;
        funcEnterPlayer = (other) => { StartCoroutine(OnEnterPos()); };
    }

    IEnumerator OnEnterPos()
    {
        while(time <= 1)
        {
            Vector3 currentValue = Vector3.Lerp(startPos, enterPos, time);
            currentValue.z = 0;
            transform.position = currentValue;
            time += Time.deltaTime;
            yield return null;
        }
    }
}