using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_GameObj : MonoBehaviour
{
    public Transform trFirst, trSecond;

    public void Set(GameObject first, GameObject second)
    {
        trFirst = first.GetComponent<Transform>();
        trSecond = second.GetComponent<Transform>();
        transform.position = new Vector3(trFirst.position.x + 2.5f, 0);
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            transform.eulerAngles = new Vector3(0, 0, 10 * (trSecond.position.y - trFirst.position.y));
            transform.position = new Vector3(transform.position.x, trFirst.position.y + (trSecond.position.y - trFirst.position.y) / 2);
        }
    }
}
