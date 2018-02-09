using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

[CustomEditor(typeof(AutosaveScene))]
public class AutosaveSceneEditor : Editor {

    AutosaveScene autosaveScene;

    [InitializeOnLoadMethod]
    public void InitUpdate() 
    { 
        EditorApplication.update += Update; 
    }

    private void OnEnable()
    {
        autosaveScene = (AutosaveScene)target;
		autosaveScene.projectPath = Application.dataPath;
    }

    private void Update()
    {
        if (autosaveScene.doAutoSave)
        {
            if (DateTime.Now.Minute >= (autosaveScene.lastSaveTimeScene.Minute + autosaveScene.intervalScene) || DateTime.Now.Minute == 59 && DateTime.Now.Second == 59)
                SaveScene();
        }
        else
            autosaveScene.isStarted = false;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Info:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Saving to:", "" + autosaveScene.projectPath);
        EditorGUILayout.LabelField("Saving scene:", "" + EditorSceneManager.GetActiveScene().name);
        GUILayout.Label("Options:", EditorStyles.boldLabel);
        autosaveScene.doAutoSave = EditorGUILayout.BeginToggleGroup("Auto save", autosaveScene.doAutoSave);
        autosaveScene.intervalScene = EditorGUILayout.IntSlider("Interval (minutes)", autosaveScene.intervalScene, 1, 10);
        if (autosaveScene.isStarted)
        {
            EditorGUILayout.LabelField("Last save:", "" + autosaveScene.lastSaveTimeScene);
        }
        EditorGUILayout.EndToggleGroup();
        autosaveScene.showMessage = EditorGUILayout.BeginToggleGroup("Show Message", autosaveScene.showMessage);
        EditorGUILayout.EndToggleGroup();
    }

    void SaveScene()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        autosaveScene.lastSaveTimeScene = DateTime.Now;
        autosaveScene.isStarted = true;
        if (autosaveScene.showMessage)
            Debug.Log("AutoSave saved: " + EditorSceneManager.GetActiveScene().name + " on " + autosaveScene.lastSaveTimeScene);
    }
}
