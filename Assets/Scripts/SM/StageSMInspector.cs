using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageSM))]
public class StageSMInspector : Editor
{
    public static int[] Stages;
    public static string[] StageNames;

    public static int currentStageIdx = 1;

    private void OnEnable()
    {
        InitStageSet();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) return;

        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = 15;
        style.alignment = TextAnchor.MiddleCenter;

        Stages = new int[Singleton.gameInfos.stageTitle.Count];

        int previous = currentStageIdx;
        if (StageNames == null)
        {
            StageNames = new string[Singleton.gameInfos.stageTitle.Count];
            foreach (var kvp in Singleton.gameInfos.stageTitle)
            {
                StageNames[kvp.Key - 1] = kvp.Value;
            }
        }

        currentStageIdx = EditorGUILayout.Popup("Stage", currentStageIdx, StageNames);
        if (previous != currentStageIdx)
        {
            EditorPrefs.SetInt(Application.productName + "Stage", currentStageIdx + 1);
        }
    }

    void InitStageSet()
    {
        currentStageIdx = EditorPrefs.GetInt(Application.productName + "Stage", 1);
    }
}
