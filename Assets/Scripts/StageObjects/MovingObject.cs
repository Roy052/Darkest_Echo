using System.Collections;
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
    }

    public IEnumerator OnEnterPos()
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

    public IEnumerator OnExitPos()
    {
        if (Vector2.Distance(enterPos, exitPos) < 0.01f) yield break;

        while (time <= 1)
        {
            Vector3 currentValue = Vector3.Lerp(startPos, enterPos, time);
            currentValue.z = 0;
            transform.position = currentValue;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
