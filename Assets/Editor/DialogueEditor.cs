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
        d.playerIndex = EditorGUILayout.IntField(new GUIContent("Player Input Index", "What players input will count on this dialogue. 0 = Both. 1 = Player 1. 2 = Player 2."), d.playerIndex);

        EditorGUILayout.LabelField("UI Settings", EditorStyles.boldLabel);
        d.npcName = EditorGUILayout.TextField(new GUIContent("NPC Name", "Will be displayed at the top as the name."), d.npcName);
        d.hide = EditorGUILayout.Toggle(new GUIContent("Hide", "Hide settings to save space."), d.hide);
        if (!d.hide)
        {
            d.npcSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("NPC Sprite", "Character portrait."), d.npcSprite, typeof(Sprite), false);
            d.rightAligned = EditorGUILayout.Toggle(new GUIContent("Right Aligned", "Choose what side the portrait will be on."), d.rightAligned);

            EditorGUILayout.Space();
            SerializedProperty sentences = serializedObject.FindProperty("sentences");
            EditorGUILayout.PropertyField(sentences, new GUIContent("Text Boxes", "Each text box is displayed independently. Do not write text longer than can be in the UI boxes."), true);
            SerializedProperty audioClips = serializedObject.FindProperty("audioClips");
            EditorGUILayout.PropertyField(audioClips, new GUIContent("Audio Clips", "Each audio clip is bound to the text box with the same index. So audio clip 2 will play with text box 2."), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Between Dialogues", EditorStyles.boldLabel);
            d.waitTime = EditorGUILayout.FloatField(new GUIContent("Wait Time", "This timer starts after all the text boxes are read. After the set amount of time (in seconds) the next dialogue component will start."), d.waitTime);
            d.moveCamera = EditorGUILayout.Toggle(new GUIContent("Move Camera", "The camera will move after all the text boxes are read. Keep in mind that you might want wait time with this to prevent text appearing while the camera is moving."), d.moveCamera);
            if (d.moveCamera)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Camera Movement", EditorStyles.boldLabel);
                d.moveCameraX = EditorGUILayout.FloatField(new GUIContent("Move Camera X", "How many units to move the camera on the X axis."), d.moveCameraX);
                d.moveCameraSpeed = EditorGUILayout.FloatField(new GUIContent("Move Camera Speed", "What speed to move the camera at. This value is inverted, higher values means a slower speed."), d.moveCameraSpeed);
                d.moveCameraWait = EditorGUILayout.FloatField(new GUIContent("Move Camera Wait", "How long to wait before snapping back to normal."), d.moveCameraWait);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Freezing", EditorStyles.boldLabel);
            d.freezeTime = EditorGUILayout.Toggle(new GUIContent("Freeze Time", "Essentially pauses the game to display the text. Might have unforseen consequences."), d.freezeTime);
            d.freezeCamera = EditorGUILayout.Toggle(new GUIContent("Freeze Camera", "Stops the camera from moving, and in turn prevents the players from leaving the view."), d.freezeCamera);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scroll Speed", EditorStyles.boldLabel);
            d.overrideSpeed = EditorGUILayout.Toggle(new GUIContent("Override Speed", "Choose to use the global speed value on the DialogueManager or a custom one for this dialogue."), d.overrideSpeed);
            if (d.overrideSpeed)
            {
                d.scrollSpeed = EditorGUILayout.FloatField(new GUIContent("Scroll Speed", "The speed in hundreds of a second for each character to appear."), d.scrollSpeed);
            }

            EditorGUILayout.Space();
            d.debug = EditorGUILayout.Toggle(new GUIContent("Display Debug", "Displays debug variables. Useful for debugging."), d.debug);
            if (d.debug)
                base.OnInspectorGUI();
        }
    }
}
