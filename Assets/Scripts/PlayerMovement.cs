using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed;

    public enum EnumFoot
    {
        Left,
        Right
    }

    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject stopPrefab;
    private float footprintSpacer;
    private float stopTime;
    private bool isMoving;
    private bool isStop;
    private bool isSneaking;
    private Vector3 lastFootprint;
    private EnumFoot whichFoot;
    private AudioSource audioSrc;

    private void Start()
    {
        mainCamera = Camera.main;
        moveSpeed = 4.5f;
        footprintSpacer = 2.0f;
        stopTime = 0;
        isMoving = false;
        isStop = false;
        isSneaking = false;
        audioSrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Player sneaking logic with left shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 2.0f;
            footprintSpacer = 1.0f;
            audioSrc.mute = true;
            isSneaking = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 4.5f;
            footprintSpacer = 2.0f;
            audioSrc.mute = false;
            isSneaking = false;
        }

        // Player movement input logic
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            // Player is moving with keyboard input
            isMoving = true;
            moveDirection = new Vector3(x, y, 0).normalized;
            transform.up = moveDirection;
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }
        else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && targetPosition != transform.position)
        {
            // Player is moving with mouse input
            isMoving = true;
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            moveDirection = (targetPosition - transform.position).normalized;
            transform.up = moveDirection;
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }

        // Footstep decal logic
        if (isMoving)
        {
            stopTime = 0;
            isStop = false;
            var movingDistance = Vector3.Distance(lastFootprint, transform.position);
            if (movingDistance >= footprintSpacer)
            {
                if (whichFoot == EnumFoot.Left)
                {
                    SpawnDecal(leftPrefab, 0.3f);
                    whichFoot = EnumFoot.Right;
                }
                else if (whichFoot == EnumFoot.Right)
                {
                    SpawnDecal(rightPrefab, 0.3f);
                    whichFoot = EnumFoot.Left;
                }

                lastFootprint = transform.position;
            }
        }
        else
        {
            //Stop decal logic
            if (!isStop)
            {
                stopTime += Time.deltaTime;
                if (stopTime >= 1.5f)
                {
                    isStop = true;
                    SpawnDecal(stopPrefab, 0.5f);
                }
            }
        }

        isMoving = false;
    }

    private void SpawnDecal(GameObject prefab, float stepWidth)
    {
        Vector3 stepOffset;
        if (prefab != stopPrefab)
        {
            //Left and right step each
            var decal = Instantiate(prefab);
            if (prefab == leftPrefab)
                stepOffset = -transform.right;
            else
                stepOffset = transform.right;
            decal.transform.position = transform.position + stepOffset * stepWidth;
            decal.transform.up = moveDirection;
            audioSrc.Play();
        }
        else
        {
            //Stop
            if (whichFoot == EnumFoot.Left)
            {
                SpawnDecal(leftPrefab, stepWidth);
                StartCoroutine(DelayedStep(rightPrefab));
            }
            else
            {
                SpawnDecal(rightPrefab, stepWidth);
                StartCoroutine(DelayedStep(leftPrefab));
            }
        }
    }

    public bool IsStop()
    {
        if (isStop) return true;
        return false;
    }
    
    public bool IsSneaking()
    {
        if (isSneaking) return true;
        return false;
    }
    
    private IEnumerator DelayedStep(GameObject prefab)
    {
        yield return new WaitForSeconds(0.2f);
        SpawnDecal(prefab, 0.5f);
    }
}