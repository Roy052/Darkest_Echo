using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Comparers;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 moveDir = Vector3.zero;
    private float moveSpeed;

    public enum EnumFoot
    {
        Left,
        Right
    }

    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject stopPrefab;
    public GameObject soundWavePrefab;
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

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Player movement input logic
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            // Player is moving with keyboard input
            isMoving = true;
            moveDir = new Vector3(x, y, 0).normalized;
            transform.up = moveDir;
            transform.position += moveSpeed * Time.deltaTime * moveDir;
        }
        else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && targetPosition != transform.position)
        {
            // Player is moving with mouse input
            isMoving = true;
            targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            moveDir = (targetPosition - transform.position).normalized;
            transform.up = moveDir;
            transform.position += moveSpeed * Time.deltaTime * moveDir;
        }
    }
    
    private void SpawnDecal(GameObject prefab, float stepWidth)
    {
        if (prefab != stopPrefab)
        {
            //Left and right step each
            Vector3 stepOffset;
            var decal = Instantiate(prefab);
            if (prefab == leftPrefab)
                stepOffset = -transform.right;
            else
                stepOffset = transform.right;
            decal.transform.position = transform.position + stepOffset * stepWidth;
            decal.transform.up = moveDir;
            audioSrc.Play();
            if (Mathf.Approximately(stepWidth, 0.3f))
            {
                var soundWave = Instantiate(soundWavePrefab);
                soundWave.transform.position = transform.position + stepOffset * stepWidth;
                if (isSneaking)
                {
                    soundWave.GetComponent<TrailRenderer>().startColor = new Color(1f, 1f, 1f, 0.5f);
                    soundWave.GetComponent<TrailRenderer>().endColor = new Color(1f, 1f, 1f, 0.5f);
                    soundWave.GetComponent<SoundWave>().fadeDuration = 0.5f;
                }
            }
        }
        else
        {
            //Stop
            if (whichFoot == EnumFoot.Left)
            {
                SpawnDecal(leftPrefab, stepWidth);
                StartCoroutine(DelayedStep(rightPrefab, stepWidth));
            }
            else
            {
                SpawnDecal(rightPrefab, stepWidth);
                StartCoroutine(DelayedStep(leftPrefab, stepWidth));
            }
        }
    }
    
    private IEnumerator DelayedStep(GameObject prefab, float stepWidth)
    {
        yield return new WaitForSeconds(0.2f);
        SpawnDecal(prefab, stepWidth);
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

}