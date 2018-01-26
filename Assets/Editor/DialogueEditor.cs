using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor {

    Dialogue d;

    private void OnEnable()
    {
        d = (Dialogue)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);
        d.playerIndex = EditorGUILayout.IntField(new GUIContent("Player Input Index", "What players input will count on this dialogue. 0 = Both. 1 = Player 1. 2 = Player 2. This also determines the placement of the dialogue box. 0 = Middle, 1 = Top Left, 2 = Bottom Right."), d.playerIndex);

        EditorGUILayout.LabelField("UI Settings", EditorStyles.boldLabel);
        d.npcName = EditorGUILayout.TextField(new GUIContent("NPC Name", "Will be displayed at the top as the name."), d.npcName);
        d.hide = EditorGUILayout.Toggle(new GUIContent("Hide", "Hide settings to save space."), d.hide);
        if (!d.hide)
        {
            d.npcSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("NPC Sprite", "Character portrait."), d.npcSprite, typeof(Sprite), false);

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            SerializedProperty sentences = serializedObject.FindProperty("sentences");
            EditorGUILayout.PropertyField(sentences, new GUIContent("Text Boxes", "Each text box is displayed independently. Do not write text longer than can be in the UI boxes."), true);

            d.voiceOver = EditorGUILayout.Toggle(new GUIContent("Voice Over", "Play audio clips with every text box."), d.voiceOver);
            if (d.voiceOver)
            {
                SerializedProperty audioClips = serializedObject.FindProperty("audioClips");
                EditorGUILayout.PropertyField(audioClips, new GUIContent("Audio Clips", "Each audio clip is bound to the text box with the same index. So audio clip 2 will play with text box 2."), true);
            }

            if(EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            d.typeSounds = EditorGUILayout.Toggle(new GUIContent("Type Sounds", "Play auio clips with every character appearing."), d.typeSounds);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Between Dialogues", EditorStyles.boldLabel);
            d.waitTime = EditorGUILayout.FloatField(new GUIContent("Wait Time", "[DO NOT SET TO 0] This timer starts after all the text boxes are read. After the set amount of time (in seconds) the next dialogue component will start."), d.waitTime);
            d.moveCamera = EditorGUILayout.Toggle(new GUIContent("Move Camera", "The camera will move after all the text boxes are read. Keep in mind that you might want wait time with this to prevent text appearing while the camera is moving."), d.moveCamera);
            if (d.moveCamera)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Camera Movement", EditorStyles.boldLabel);
                d.moveCameraX = EditorGUILayout.FloatField(new GUIContent("Move Camera X", "How many units to move the camera on the X axis."), d.moveCameraX);
                d.moveCameraSpeed = EditorGUILayout.FloatField(new GUIContent("Move Camera Speed", "What speed to move the camera at. This value is inverted, higher values means a slower speed."), d.moveCameraSpeed);
                d.moveCameraWait = EditorGUILayout.FloatField(new GUIContent("Move Camera Wait", "[DO NOT SET TO 0] How long to wait before snapping back to normal."), d.moveCameraWait);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fading", EditorStyles.boldLabel);
            d.fadeIn = EditorGUILayout.Toggle(new GUIContent("Fade In", "Wether this dialogue box will fade in."), d.fadeIn);
            d.fadeOut = EditorGUILayout.Toggle(new GUIContent("Fade Out", "Wether this dialogue box will fade out."), d.fadeOut);
            if(d.fadeIn || d.fadeOut)
                d.fadeTime = EditorGUILayout.FloatField(new GUIContent("Fade Time", "[DO NOT SET TO 0] The fading time in seconds. Applies to both fade in and out."), d.fadeTime);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Freezing", EditorStyles.boldLabel);
            d.freezeTime = EditorGUILayout.Toggle(new GUIContent("Freeze Time", "[DO NOT SET TO 0] Essentially pauses the game to display the text. Might have unforseen consequences."), d.freezeTime);
            d.freezeCamera = EditorGUILayout.Toggle(new GUIContent("Freeze Camera", "Stops the camera from moving, and in turn prevents the players from leaving the view."), d.freezeCamera);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Speed", EditorStyles.boldLabel);
            d.overrideSpeed = EditorGUILayout.Toggle(new GUIContent("Override Speed", "Choose to use the global speed value on the DialogueManager or a custom one for this dialogue."), d.overrideSpeed);
            if (d.overrideSpeed)
            {
                d.typeSpeed = EditorGUILayout.FloatField(new GUIContent("Type Speed", "[DO NOT SET TO 0] The speed in hundreds of a second for each character to appear."), d.typeSpeed);
            }

            EditorGUILayout.Space();
            d.debug = EditorGUILayout.Toggle(new GUIContent("Display Debug", "Displays debug variables. Useful for debugging."), d.debug);
            if (d.debug)
                base.OnInspectorGUI();
        }
    }
}
