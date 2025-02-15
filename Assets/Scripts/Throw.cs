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
        if (Singleton.stageSM.objEscapeMenu.activeSelf)
            return;

        if (Singleton.player.canThrow == false) return;

        targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
        var sqrLen = Vector3.Distance(targetPosition, transform.position);
        //Debug.Log(sqrLen);
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