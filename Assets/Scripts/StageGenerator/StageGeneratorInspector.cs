using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageGenerator))]
public class StageGeneratorInspector : Editor
{
    public static int[] Stages;
    public static string[] StageNames;

    int idxMap;

    private void OnEnable()
    {
        if (EditorApplication.isPlaying)
            InitPlaySet();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = 15;
        style.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.Separator();
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Immortal");
            GUILayout.FlexibleSpace();
            StageGenerator.isImmortal = EditorGUILayout.Toggle(StageGenerator.isImmortal, GUILayout.Width(16f));
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("IsImmortal", StageGenerator.isImmortal);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Stages = new int[Singleton.gameInfos.stageTitle.Count];

            int previous = StageGenerator.stageIdx;
            if (StageNames == null)
            {
                StageNames = new string[Singleton.gameInfos.stageTitle.Count];
                foreach(var kvp in Singleton.gameInfos.stageTitle)
                { 
                    StageNames[kvp.Key - 1] = kvp.Value;
                }
            }

            idxMap = EditorGUILayout.Popup("Stage", idxMap, StageNames);
            if (previous != idxMap)
                EditorPrefs.SetInt("EditorPlayStage", idxMap);

            GUILayout.Space(6f);
            if (GUILayout.Button("Save", style, GUILayout.Height(30)))
                Save();

            if (GUILayout.Button("Load", style, GUILayout.Height(30)))
                Load();
        }
    }

    void InitPlaySet()
    {
        StageGenerator.isImmortal = EditorPrefs.GetBool("IsImmortal", false);
    }

    public void Save()
    {
        StageGenerator stageGenerator = GameObject.Find("StageGenerator").GetComponent<StageGenerator>();
        stageGenerator.SaveStageData(idxMap);
        AssetDatabase.Refresh();
    }

    public void Load()
    {
        StageGenerator stageGenerator = GameObject.Find("StageGenerator").GetComponent<StageGenerator>();
        stageGenerator.LoadStageData(idxMap);
        AssetDatabase.Refresh();
    }

    public void ResetExist()
    {
        Debug.Log("Reset");
    }
}