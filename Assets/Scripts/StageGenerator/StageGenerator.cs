using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageData
{
    public SerializableTransform player;
    public SerializableTransform endZone;

    public List<SerializableTransform> walls = new List<SerializableTransform>();

    public List<int> enemyTypes = new List<int>();
    public List<int> objectTypes = new List<int>();
    public List<SerializableTransform> enemys = new List<SerializableTransform>();
    public List<SerializableScoutPos> scoutPoses = new List<SerializableScoutPos>();
    public List<SerializableTransform> objects = new List<SerializableTransform>();
    public List<SerializableVector2> movingObjectPoses = new List<SerializableVector2>();
}

[System.Serializable]
public class SerializableTransform
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public SerializableTransform()
    {
        this.position = Vector3.zero;
        this.rotation = Vector3.zero;
        this.scale = Vector3.zero;
    }

    public SerializableTransform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public SerializableTransform(Transform transform)
    {
        this.position = transform.position;
        this.rotation = transform.eulerAngles;
        this.scale = transform.localScale;
    }
}

[System.Serializable]
public class SerializableScoutPos
{
    public float posAX;
    public float posAY;
    public float posBX;
    public float posBY;

    public SerializableScoutPos()
    {
        posAX = 0;
        posAY = 0;
        posBX = 0;
        posBY = 0;
    }

    public SerializableScoutPos(Vector2 pointA, Vector2 pointB)
    {
        posAX = pointA.x;
        posAY = pointA.y;
        posBX = pointB.x;
        posBY = pointB.y;
    }

    public SerializableScoutPos(float posAX, float posAY, float posBX, float posBY)
    {
        this.posAX = posAX;
        this.posAY = posAY;
        this.posBX = posBX;
        this.posBY = posBY;
    }
}

[System.Serializable]
public class SerializableVector2
{
    public float posX;
    public float posY;

    public SerializableVector2()
    {
        posX = 0;
        posY = 0;
    }

    public SerializableVector2(Vector2 point)
    {
        posX = point.x;
        posY = point.y;
    }

    public SerializableVector2(float posX, float posY)
    {
        this.posX = posX;
        this.posY = posY;
    }
}

public class SerializableObject
{
    public int type;
    public SerializableTransform transform = new SerializableTransform();
}

public class SerializableList<T>
{
    public List<T> list;
}

public class StageGenerator : Singleton
{
    string path;
    public static readonly string fileName = "StageData";

    [SerializeField] private StageData stageData;

    public Transform playerParent;
    public Transform endZoneParent;
    public Transform wallsParent;
    public Transform enemysParent;
    public Transform objectsParent;

    public GameObject wallPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] objectPrefab;

    private void Awake()
    {
        stageGenerator = this;
    }

    private void OnDestroy()
    {
        stageGenerator = null;
    }

    public void SaveStageData(int stageNum)
    {
        stageData = new StageData();

        //Player
        for (int i = 0; i < playerParent.childCount; i++)
        {
            if (playerParent.GetChild(i).gameObject.activeSelf)
            {
                stageData.player = new SerializableTransform(playerParent.GetChild(i).transform);
                break;
            }
        }
        
        if(stageData.player == null)
        {
            Debug.LogError("Player No Exist Error");
            return;
        }

        //EndZone
        for (int i = 0; i < endZoneParent.childCount; i++)
        {
            if (endZoneParent.GetChild(i).gameObject.activeSelf)
            {
                stageData.endZone = new SerializableTransform(endZoneParent.GetChild(i).transform);
                break;
            }
        }

        if (stageData.endZone == null)
        {
            Debug.LogError("EndZone No Exist Error");
            return;
        }

        //Wall
        for (int i = 0; i < wallsParent.childCount; i++)
        {
            if (wallsParent.GetChild(i).gameObject.activeSelf)
                stageData.walls.Add(new SerializableTransform(wallsParent.GetChild(i).transform));
        }

        //Enemy
        for(int i = 0; i < enemysParent.childCount; i++)
        {
            if (enemysParent.GetChild(i).gameObject.activeSelf)
            {
                EnemyAI enemyAI = enemysParent.GetChild(i).GetComponent<EnemyAI>();
                stageData.enemyTypes.Add((int) enemyAI.enemyType);
                stageData.enemys.Add(new SerializableTransform(enemysParent.GetChild(i).transform));
                if(enemyAI.enemyType == EnemyType.Scout)
                    stageData.scoutPoses.Add(new SerializableScoutPos(enemyAI.pointA, enemyAI.pointB));
            }
                
        }

        //Object
        for (int i = 0; i < objectsParent.childCount; i++)
        {
            if (objectsParent.GetChild(i).gameObject.activeSelf)
            {
                StageObject stageObject = objectsParent.GetChild(i).gameObject.GetComponent<StageObject>();
                stageData.objectTypes.Add((int) stageObject.type);
                stageData.objects.Add(new SerializableTransform(objectsParent.GetChild(i).transform));
                if (stageObject.type == StageObjectType.MovingObject)
                    stageData.movingObjectPoses.Add(new SerializableVector2((stageObject as MovingObject).enterPos));
            }
                
        }

        string json = JsonUtility.ToJson(stageData);
        path = Application.dataPath + "/StageDatas";
        Debug.Log(path);
        File.WriteAllText(path + "/" + fileName + (stageNum + 1), json);
        Debug.Log(json);
    }

    public void LoadStageData(int stageNum)
    {
        path = Application.dataPath + "/StageDatas";
        if(File.Exists(path + "/" + fileName + (stageNum + 1)) == false){
            Debug.LogError("No File Exists");
            return;
        }
        string data = File.ReadAllText(path + "/" + fileName + (stageNum + 1));
        Debug.Log(data);
        StageData stageData = JsonUtility.FromJson<StageData>(data);

        if (stageData.player == null || stageData.endZone == null)
        {
            Debug.LogError("Stage Data No Exist Error");
            return;
        }

        //Player
        Transform trPlayer = playerParent.GetChild(0).gameObject.transform;
        trPlayer.position = stageData.player.position;
        trPlayer.eulerAngles = stageData.player.rotation;
        trPlayer.localScale = stageData.player.scale;
        trPlayer.SetAsFirstSibling();

        //EndZone
        Transform trEndZone = endZoneParent.GetChild(0).gameObject.transform;
        trEndZone.position = stageData.endZone.position;
        trEndZone.eulerAngles = stageData.endZone.rotation;
        trEndZone.localScale = stageData.endZone.scale;
        trEndZone.SetAsFirstSibling();

        //Wall
        int count = wallsParent.childCount;
        for(int i = 0; i < stageData.walls.Count; i++)
        {
            if (i > count - 1)
            {
                GameObject objWall = Instantiate(wallPrefab, wallsParent);
                objWall.name = $"Wall {i}";
                objWall.SetActive(true);

                Transform trWall = objWall.transform;
                trWall.position = stageData.walls[i].position;
                trWall.eulerAngles = stageData.walls[i].rotation;
                trWall.localScale = stageData.walls[i].scale;
                trWall.SetAsFirstSibling();
            }
            else
            {
                Transform trWall = wallsParent.GetChild(i);
                trWall.position = stageData.walls[i].position;
                trWall.eulerAngles = stageData.walls[i].rotation;
                trWall.localScale = stageData.walls[i].scale;
                trWall.SetAsFirstSibling();
            }
        }

        while(wallsParent.childCount > stageData.walls.Count)
        {
            DestroyImmediate(wallsParent.GetChild(wallsParent.childCount - 1).gameObject);
        }

        //Enemy
        while(enemysParent.childCount > 0)
        {
            DestroyImmediate(enemysParent.GetChild(0).gameObject);
        }

        int scoutCount = 0;
        for(int i = 0; i < stageData.enemys.Count; i++)
        {
            GameObject objEnemy = Instantiate(enemyPrefab[stageData.enemyTypes[i]], enemysParent);
            objEnemy.name = $"Enemy {i}";
            objEnemy.SetActive(true);

            Transform trEnemy = objEnemy.transform;
            trEnemy.position = stageData.enemys[i].position;
            trEnemy.eulerAngles = stageData.enemys[i].rotation;
            trEnemy.localScale = stageData.enemys[i].scale;
            trEnemy.SetAsFirstSibling();

            if(stageData.enemyTypes[i] == (int)EnemyType.Scout)
            {
                EnemyAI enemyAI = objEnemy.GetComponent<EnemyAI>();
                enemyAI.pointA = new Vector2(stageData.scoutPoses[scoutCount].posAX, stageData.scoutPoses[scoutCount].posAY);
                enemyAI.pointB = new Vector2(stageData.scoutPoses[scoutCount].posBX, stageData.scoutPoses[scoutCount].posBY);
                scoutCount++;
            }
        }

        //Object
        while (objectsParent.childCount > 0)
        {
            DestroyImmediate(objectsParent.GetChild(0).gameObject);
        }

        int movingObjectCount = 0;
        for (int i = stageData.objects.Count - 1; i >= 0; i--)
        {
            GameObject objObject = Instantiate(objectPrefab[stageData.objectTypes[i]], objectsParent);
            objObject.SetActive(true);

            Transform trObject = objObject.transform;
            trObject.position = stageData.objects[i].position;
            trObject.eulerAngles = stageData.objects[i].rotation;
            trObject.localScale = stageData.objects[i].scale;
            trObject.SetAsFirstSibling();

            if (stageData.objectTypes[i] == (int)StageObjectType.MovingObject)
            {
                MovingObject movingObject = objObject.GetComponent<MovingObject>();
                movingObject.enterPos = new Vector2(stageData.movingObjectPoses[movingObjectCount].posX, stageData.movingObjectPoses[movingObjectCount].posY);
                movingObjectCount++;
            }
        }
    }

    public static bool isImmortal;
}

