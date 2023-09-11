using UnityEngine;

public enum StageObjectType
{
    AlertDevice = 0,
    StageArea = 1,
    Trap = 2,
    Water = 3,
}

public class StageObject : MonoBehaviour
{
    public StageObjectType type;
}
