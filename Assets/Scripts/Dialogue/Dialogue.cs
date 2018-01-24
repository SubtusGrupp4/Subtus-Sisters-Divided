using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour {
    [Header("Input Settings")]
    public int playerIndex = 0;

    [Header("UI Settings")]
    public bool rightAligned = false;
    public string npcName;
    public Sprite npcSprite;

    [Header("Dialogue Boxes")]
    [TextArea(3, 10)]
    public string[] sentences;
    public bool voiceOver = false;
    public AudioClip[] audioClips;
    public bool typeSounds = true;
    //public AudioClip[] typingSounds;
    [Header("Between Dialogues")]
    public float waitTime = 0f;
    public bool moveCamera = false;
    [Header("Freezing")]
    public bool freezeTime = false;
    public bool freezeCamera = false;
    [Header("Camera Movement")]
    public float moveCameraX = 0f;
    public float moveCameraSpeed = 0f;
    public float moveCameraWait = 0f;

    [HideInInspector]
    public bool hide = false;
    [HideInInspector]
    public bool debug = false;
    [HideInInspector]
    public bool overrideSpeed = false;
    public float typeSpeed = 8f;

    private void Start()
    {
        // Prevent dividing by 0
        if(moveCameraSpeed <= 1f)
            moveCameraSpeed = 1f;
    }
}
