using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum EnumFoot
    {
        Left,
        Right
    }

    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 moveDir = Vector3.zero;
    private float moveSpeed;
    private float clapPower;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject stopPrefab;
    public GameObject soundWavePrefab;
    private float footprintSpacer;
    private float movementTimer;
    private float stopTime;
    private bool isMoving;
    private bool isStop;
    private bool isSneaking;
    private bool isClapping;
    private EnumFoot whichFoot;
    private AudioSource audioSrc;


    public void Awake()
    {
        mainCamera = Camera.main;
        moveSpeed = 3.5f;
        footprintSpacer = 0.5f;
        movementTimer = 0;
        clapPower = 0;
        stopTime = 0;
        isMoving = false;
        isStop = false;
        isSneaking = false;
        isClapping = false;
        whichFoot = EnumFoot.Left;
        audioSrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Player sneaking logic with left shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 1.5f;
            isSneaking = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 3.5f;
            isSneaking = false;
        }
        
        // Clap logic with space bar
        if (!isSneaking)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
            {
                clapPower += Time.deltaTime;
            }
            else if (Input.GetKeyUp(KeyCode.Space) && !isClapping)
            {
                isClapping = true;
                clapPower = Mathf.Clamp(clapPower, 0f, 1f);
                audioSrc.volume = Mathf.Lerp(0.5f, 1f, clapPower);
                clapPower = Mathf.Lerp(1f, 3f, clapPower);
                SoundWaveGenerator.instance.SpawnSoundWave(isSneaking, isClapping, transform.position);
                audioSrc.Play();
                clapPower = 0;
                StartCoroutine(ClapDelay());
            }
        }

        // Footstep decal logic
        if (isMoving)
        {
            stopTime = 0;
            movementTimer += Time.deltaTime;
            isStop = false;
            if (movementTimer >= footprintSpacer)
            {
                movementTimer = 0;
                if (whichFoot == EnumFoot.Left)
                {
                    SpawnDecal(leftPrefab, 0.1f);
                    whichFoot = EnumFoot.Right;
                }
                else if (whichFoot == EnumFoot.Right)
                {
                    SpawnDecal(rightPrefab, 0.1f);
                    whichFoot = EnumFoot.Left;
                }
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
                    movementTimer = 0;
                    SpawnDecal(stopPrefab, 0.2f);
                }
            }
        }
    }

    private void FixedUpdate()
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
        else
        {
            isMoving = false;
        }
    }

    private void SpawnDecal(GameObject prefab, float stepWidth)
    {
        if (prefab != stopPrefab)
        {
            //Left and right step each
            float stepOffset;
            var decal = Instantiate(prefab);
            if (prefab == leftPrefab)
                stepOffset = -1f;
            else
                stepOffset = 1f;
            decal.transform.position = transform.position + stepOffset * stepWidth * transform.right;
            decal.transform.up = moveDir;
            decal.transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, stepOffset * 5f));
            if (Mathf.Approximately(stepWidth, 0.1f))
                SoundWaveGenerator.instance.SpawnSoundWave(isSneaking, false,
                    transform.position + stepOffset * stepWidth * transform.right);
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

    private IEnumerator ClapDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isClapping = false;
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

    public bool IsClapping()
    {
        if (isClapping) return true;
        return false;
    }

    public float GetFadeDuration()
    {
        return clapPower;
    }
}