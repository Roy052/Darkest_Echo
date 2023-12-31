using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(StageGenerator))]
public class StageGeneratorInspector : Editor
{
    public StageGenerator stageGenerator;
    public static int[] Stages;
    public static string[] StageNames;

    int currentStageIdx;

    private void OnEnable()
    {
        stageGenerator = (StageGenerator)target;

        if (EditorApplication.isPlaying)
            InitPlaySet();
        else
            InitEditorSet();
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

            int previous = currentStageIdx;
            if (StageNames == null)
            {
                StageNames = new string[Singleton.gameInfos.stageTitle.Count];
                foreach(var kvp in Singleton.gameInfos.stageTitle)
                { 
                    StageNames[kvp.Key - 1] = kvp.Value;
                }
            }

            currentStageIdx = EditorGUILayout.Popup("Stage", currentStageIdx, StageNames);
            if (previous != currentStageIdx)
                EditorPrefs.SetInt(Application.productName + "EditorStage", currentStageIdx);

            GUILayout.Space(6f);
            if (GUILayout.Button("Save", style, GUILayout.Height(30)))
                Save();

            if (GUILayout.Button("Load", style, GUILayout.Height(30)))
                Load();

            if (GUILayout.Button("Reset Objects", style, GUILayout.Height(30)))
                ResetObjects();
        }
    }

    void InitPlaySet()
    {
        StageGenerator.isImmortal = EditorPrefs.GetBool("IsImmortal", false);
    }

    void InitEditorSet()
    {
        currentStageIdx = EditorPrefs.GetInt(Application.productName + "EditorStage", 0);
    }

    public void Save()
    {
        StageGenerator stageGenerator = GameObject.Find("StageGenerator").GetComponent<StageGenerator>();
        stageGenerator.SaveStageData(currentStageIdx);
        AssetDatabase.Refresh();
    }

    public void Load()
    {
        StageGenerator stageGenerator = GameObject.Find("StageGenerator").GetComponent<StageGenerator>();
        stageGenerator.LoadStageData(currentStageIdx);
        AssetDatabase.Refresh();
    }

    public void ResetObjects()
    {
        while (stageGenerator.wallsParent.childCount > 0)
            DestroyImmediate(stageGenerator.wallsParent.GetChild(0).gameObject);

        while (stageGenerator.enemysParent.childCount > 0)
            DestroyImmediate(stageGenerator.enemysParent.GetChild(0).gameObject);

        while (stageGenerator.objectsParent.childCount > 0)
            DestroyImmediate(stageGenerator.objectsParent.GetChild(0).gameObject);
    }
}
#endif