using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 moveDirection = Vector3.zero;
    private float moveSpeed;
    
    public enum EnumFoot
    {
        Left,
        Right,
    }

    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject stop = null;
    public float footprintSpacer = 2.0f;
    private float stopTime;
    private bool isMoving;
    private Vector3 lastFootprint;
    private EnumFoot whichFoot;
    private AudioSource audioSrc;

    private void Start()
    {
        mainCamera = Camera.main;
        moveSpeed = 4.5f;
        stopTime = 0;
        isMoving = false;
        audioSrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        
        // Player movement input logic
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
            float movingDistance = Vector3.Distance(lastFootprint, transform.position);
            if (movingDistance >= footprintSpacer)
            {
                if (whichFoot == EnumFoot.Left)
                {
                    SpawnDecal(leftPrefab);
                    whichFoot = EnumFoot.Right;
                }
                else if (whichFoot == EnumFoot.Right)
                {
                    SpawnDecal(rightPrefab);
                    whichFoot = EnumFoot.Left;
                }
                lastFootprint = transform.position;
            }
        }
        else
        {
            //추가필요
            stopTime += Time.deltaTime;
            if (stopTime >= 2f)
            {
                SpawnDecal(stop);
                stopTime = 0f;
            }
        }
        
        isMoving = false;
    }

    private void SpawnDecal(GameObject prefab)
    {
        Vector3 stepOffset;
        if (prefab)
        {
            //Left and right step each
            GameObject decal = Instantiate(prefab);
            if (prefab == leftPrefab)
            {
                stepOffset = -transform.right;
            }
            else
            {
                stepOffset = transform.right;
            }
            decal.transform.position = transform.position + (stepOffset * 0.3f);
            decal.transform.up = moveDirection;
            audioSrc.Play();
        }
        else
        {
            //Stop
            GameObject decal = Instantiate(leftPrefab);
            stepOffset = -transform.right;
            decal.transform.position = transform.position + (stepOffset * 0.5f);
            decal.transform.up = moveDirection;
            
            decal = Instantiate(rightPrefab);
            stepOffset = transform.right;
            decal.transform.position = transform.position + (stepOffset * 0.5f);
            decal.transform.up = moveDirection;
        }
        
    }
}
