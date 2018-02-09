using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

public class AutosaveScene : MonoBehaviour 
{
    public bool doAutoSave = false;
    public bool showMessage = true;
    public bool isStarted = false;
    public int intervalScene = 2;
    public DateTime lastSaveTimeScene = DateTime.Now;

    public string projectPath;
}
