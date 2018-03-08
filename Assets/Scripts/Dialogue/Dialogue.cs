using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerIndex
{
    Both, Player1, Player2
}

// This script serves as a single dialogue box
// It will be read in the DialogueManager and displayed on the UI
// TODO: Add tooltips on what everything does
public class Dialogue : MonoBehaviour {
    [Header("Input Settings")]
    public PlayerIndex playerIndex;

    [Header("UI Settings")]
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
    [Header("Fading")]
    public bool fadeIn = false;
    public bool fadeOut = false;
    public float fadeTime = 1f;
    [Header("Freezing")]
    public bool freezeTime = false;
    public bool freezeCamera = false;
    [Header("Camera Movement")]
    public float moveCameraX = 0f;
    public float moveCameraSpeed = 1f;
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
        GameManager.instance.PreventZero(moveCameraSpeed, 1f);
    }
}
