using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageData
{
    public SerializableTransform player;

    public List<SerializableTransform> walls = new List<SerializableTransform>();
    public List<SerializableObject> enemys = new List<SerializableObject>();
    public List<SerializableObject> objects = new List<SerializableObject>();
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
    readonly string fileName = "StageData";

    [SerializeField] private StageData stageData;

    public Transform playerParent;
    public Transform wallsParent;
    public Transform enemysParent;
    public Transform objectsParent;

    public void SaveStageData(int stageNum)
    {
        stageData = new StageData();
        for (int i = 0; i < playerParent.childCount; i++)
        {
            if (playerParent.GetChild(0).gameObject.activeSelf)
            {
                stageData.player = new SerializableTransform(playerParent.GetChild(0).transform);
                break;
            }
        }
        
        if(stageData.player == null)
        {
            Debug.LogError("Player No Exist Error");
            return;
        }

        for(int i = 0; i < wallsParent.childCount; i++)
        {
            stageData.walls.Add(new SerializableTransform(wallsParent.GetChild(i).transform));
        }

        for(int i = 0; i < enemysParent.childCount; i++)
        {
            stageData.enemys.Add(new SerializableObject() 
            { type = 0, transform = new SerializableTransform(enemysParent.GetChild(i).transform) });
        }

        for (int i = 0; i < enemysParent.childCount; i++)
        {
            stageData.enemys.Add(new SerializableObject()
            { type = (int) enemysParent.GetChild(i).GetComponent<StageObject>().type, 
                transform = new SerializableTransform(enemysParent.GetChild(i).transform) });
        }

        string json = JsonUtility.ToJson(stageData);
        path = Application.persistentDataPath;
        File.WriteAllText(path + "/" + fileName + stageNum, json);
        Debug.Log(json);
    }

    public void LoadStageData(int stageNum)
    {
        string data = File.ReadAllText(path + "/" + fileName + stageNum);
        Debug.Log(data);
        StageData file = JsonUtility.FromJson<StageData>(data);
        Debug.Log(file.player.position);
    }

    public static bool isImmortal;
}

