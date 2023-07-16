using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] float destroyTime = 2f;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - startTime > destroyTime)
            Destroy(gameObject);
    }
}