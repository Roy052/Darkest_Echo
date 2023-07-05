using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;
    private bool isMoving;

    private void Start()
    {
        mainCamera = Camera.main;
        isMoving = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            isMoving = true;
            Debug.Log(targetPosition);
        }

        if (isMoving)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                // Arrived at the target position
                isMoving = false;
            }
        }
    }
}
