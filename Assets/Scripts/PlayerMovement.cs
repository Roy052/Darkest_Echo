using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum EnumFloor
    {
        Tile,
        Water
    }

    
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject stopPrefab;
    public GameObject soundWavePrefab;
    
    public bool isMoving;
    public bool isStop;
    public bool isSneaking;
    public bool isWater;
    public bool isDead;
    public bool canThrow;

    public EnumFloor currentFloor;

    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 moveDir = Vector3.zero;
    private float moveSpeed;

    private string whichFoot;
    private AudioSource audioSrc;
    private float footprintSpacer;
    private float movementTimer;
    private float stopTime;


    public void Awake()
    {
        mainCamera = Camera.main;
        moveSpeed = 3.5f;
        footprintSpacer = 0.5f;
        movementTimer = 0;
        stopTime = 0;
        isMoving = false;
        isStop = false;
        isSneaking = false;
        isDead = false;
        currentFloor = EnumFloor.Tile;
        whichFoot = "Left";
        audioSrc = GetComponent<AudioSource>();

        Singleton.player = this;
    }

    private void OnDestroy()
    {
        Singleton.player = null;
    }

    private void Update()
    {
        if (isWater || isSneaking) moveSpeed = 1.5f;
        else moveSpeed = 3.5f;

        // Player sneaking logic with left shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSneaking = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSneaking = false;
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
                if (whichFoot == "Left")
                {
                    SpawnDecal(leftPrefab, 0.1f);
                    whichFoot = "Right";
                }
                else if (whichFoot == "Right")
                {
                    SpawnDecal(rightPrefab, 0.1f);
                    whichFoot = "Left";
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
        //else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && targetPosition != transform.position)
        //{
        //    // Player is moving with mouse input
        //    isMoving = true;
        //    targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //    targetPosition.z = transform.position.z;
        //    moveDir = (targetPosition - transform.position).normalized;
        //    transform.up = moveDir;
        //    transform.position += moveSpeed * Time.deltaTime * moveDir;
        //}
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
            {
                if (isSneaking)
                    SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Sneaking,
                        transform.position + stepOffset * stepWidth * transform.right);
                else if (currentFloor == EnumFloor.Water)
                    SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Wading,
                        transform.position + stepOffset * stepWidth * transform.right);
                else
                    SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Normal,
                        transform.position + stepOffset * stepWidth * transform.right);
            }
        }
        else
        {
            //Stop
            if (whichFoot == "Left")
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        // When Player collides with water, create a sound wave 
        if (other.gameObject.CompareTag(Str.TagWater))
        {
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Wading, transform.position);
            isWater = true;
            footprintSpacer = 1f;
            currentFloor = EnumFloor.Water;
        }
        else if (other.gameObject.CompareTag(Str.TagTrap))
        {
            isDead = true;
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Dying, transform.position);
            //Singleton.stageSM.StageRestart();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // If Player is on water, he can't sneak and make player slow
        if (other.gameObject.CompareTag(Str.TagWater))
            isSneaking = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If Player is not on water, he can sneak and make player fast
        if (other.gameObject.CompareTag(Str.TagWater))
        {
            SoundWaveGenerator.instance.SpawnSoundWave(SoundWaveGenerator.WaveType.Wading, transform.position);
            isWater = false;
            footprintSpacer = 0.5f;
            currentFloor = EnumFloor.Tile;
        }
    }
    
}