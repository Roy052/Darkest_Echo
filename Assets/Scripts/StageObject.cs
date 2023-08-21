using UnityEngine;

public enum StageObjectType
{
    AlertDevice = 0,
    StageArea = 1,
}

public class StageObject : MonoBehaviour
{
    public StageObjectType type;
}
