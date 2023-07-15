using System.Collections;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private Vector3 cameraPosStart;
    private Vector3 cameraPosEnd;
    float slipFactor = 0.95f;

    public float dragSpeed = 3f;
    public float cameraPosMin = 0;
    public float cameraPosMax = 10;
    public void SetCameraPosMax(float posMax)
    {
        cameraPosMax = posMax;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        if (isDragging)
        {
            UpdateDrag();
        }
    }

    private void StartDrag()
    {
        isDragging = true;
        dragOrigin = Input.mousePosition;
        cameraPosStart = transform.position;
    }

    public Vector3 currentPosition;
    public Vector3 moveVector;
    private void UpdateDrag()
    {
        currentPosition = Input.mousePosition;
        moveVector = dragOrigin - currentPosition;
        moveVector.y = 0;

        if (transform.position.x < cameraPosMin && moveVector.x < 0) return;
        if (transform.position.x > cameraPosMax && moveVector.x > 0) return;
        transform.Translate(moveVector * dragSpeed * Time.deltaTime);

        dragOrigin = currentPosition;
    }

    private void EndDrag()
    {
        isDragging = false;
        cameraPosEnd = transform.position;
        Debug.Log(cameraPosEnd.x - cameraPosStart.x);
        StartCoroutine(Slip());
    }

    bool isMovedLeft;
    IEnumerator Slip()
    {
        isMovedLeft = cameraPosEnd.x - cameraPosStart.x < 0;
        Vector3 slipVector = new Vector3(cameraPosEnd.x - cameraPosStart.x, 0);
        slipVector *= 3;
        while(transform.position.x > cameraPosMin && transform.position.x < cameraPosMax)
        {
            if (isDragging) yield break;
            transform.Translate(slipVector * Time.deltaTime);
            slipVector *= slipFactor;
            
            yield return null;
        }
        
    }
}