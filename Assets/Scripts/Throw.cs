using System.Collections;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private Vector3 targetPosition;
    private Camera mainCamera;
    private bool isThrowing;

    private void Awake()
    {
        mainCamera = Camera.main;
        isThrowing = false;
    }

    private void Update()
    {
        targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var sqrLen = Vector3.SqrMagnitude(targetPosition - transform.position);

        // when mouse is clicked on player
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !isThrowing && sqrLen < 1.2f)
        {
            isThrowing = true;
        }
        else if (Input.GetMouseButtonUp(0) && isThrowing && targetPosition != transform.position)
        {
            var position = transform.position;
            var direction = targetPosition - position;
            direction.z = 0;
            SoundWaveGenerator.instance.ThrowSoundWave(direction, position);
            StartCoroutine(ThrowDelay());
        }
    }
    
    private IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(0.3f);
        isThrowing = false;
    }
}