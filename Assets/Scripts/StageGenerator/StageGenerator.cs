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
    public List<SerializableTransform> objects = new List<SerializableTransform>();
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

public class SerializableObject
{
    public int type;
    public SerializableTransform transform = new SerializableTransform();
}

public class SerializableList<T>
{
    public List<T> list;
}

public class StageGenerator : MonoBehaviour
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
                stageData.enemyTypes.Add(0);
                stageData.enemys.Add(new SerializableTransform(enemysParent.GetChild(i).transform));
            }
                
        }

        //Object
        for (int i = 0; i < objectsParent.childCount; i++)
        {
            if (objectsParent.GetChild(i).gameObject.activeSelf)
            {
                stageData.objectTypes.Add((int) objectsParent.GetChild(i).gameObject.GetComponent<StageObject>().type);
                stageData.objects.Add(new SerializableTransform(objectsParent.GetChild(i).transform));
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

        //Enemy
        while(enemysParent.childCount > 1)
        {
            Destroy(enemysParent.GetChild(0).gameObject);
        }

        for(int i = 0; i < stageData.enemys.Count; i++)
        {
            if (i > count - 1)
            {
                GameObject objEnemy = Instantiate(enemyPrefab[stageData.enemyTypes[i]], enemysParent);
                objEnemy.name = $"Enemy {i}";
                objEnemy.SetActive(true);

                Transform trEnemy = objEnemy.transform;
                trEnemy.position = stageData.enemys[i].position;
                trEnemy.eulerAngles = stageData.enemys[i].rotation;
                trEnemy.localScale = stageData.enemys[i].scale;
                trEnemy.SetAsFirstSibling();
            }
        }

        //Object
        while (objectsParent.childCount > 1)
        {
            Destroy(objectsParent.GetChild(0).gameObject);
        }

        for (int i = stageData.objects.Count - 1; i >= 0; i--)
        {
            if (i >= count - 1)
            {
                GameObject objObject = Instantiate(objectPrefab[stageData.objectTypes[i]], objectsParent);
                objObject.SetActive(true);

                Transform trObject = objObject.transform;
                trObject.position = stageData.objects[i].position;
                trObject.eulerAngles = stageData.objects[i].rotation;
                trObject.localScale = stageData.objects[i].scale;
                trObject.SetAsFirstSibling();
            }
        }
    }

    public static bool isImmortal;
}

