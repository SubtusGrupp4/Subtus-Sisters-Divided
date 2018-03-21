using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FMODEmitter))]
public class StudioEventEmitterEditor : Editor
{
    FMODEmitter emitter;

    public void OnEnable()
    {
        emitter = (FMODEmitter)target;
    }

    public override void OnInspectorGUI()
    {
        var ev = serializedObject.FindProperty("Event");
        var param = serializedObject.FindProperty("parameters");
        /*
        var overrideAtt = serializedObject.FindProperty("OverrideAttenuation");
        var minDistance = serializedObject.FindProperty("OverrideMinDistance");
        var maxDistance = serializedObject.FindProperty("OverrideMaxDistance");
        */

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(ev, new GUIContent("Event"));

        FMODUnity.EditorEventRef editorEvent = FMODUnity.EventManager.EventFromPath(ev.stringValue);

        /*
        if (EditorGUI.EndChangeCheck())
        {
            FMODUnity.EditorUtils.UpdateParamsOnEmitter(serializedObject, ev.stringValue);
            if (editorEvent != null)
            {
                overrideAtt.boolValue = false;
                minDistance.floatValue = editorEvent.MinDistance;
                maxDistance.floatValue = editorEvent.MaxDistance;
            }
        }
        */

        // Attenuation
        if (editorEvent != null)
        {
            {
                emitter.playOnStart = EditorGUILayout.Toggle("Play on Start", emitter.playOnStart);
                EditorGUILayout.Space();

                emitter.is3D = EditorGUILayout.Toggle("Is 3D", emitter.is3D);

                /*
                EditorGUI.BeginDisabledGroup(editorEvent == null || !editorEvent.Is3D);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Override Attenuation");
                EditorGUI.BeginChangeCheck();
                overrideAtt.boolValue = EditorGUILayout.Toggle(overrideAtt.boolValue, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck() ||
                    (minDistance.floatValue == -1 && maxDistance.floatValue == -1) // never been initialiased
                    )
                {
                    minDistance.floatValue = editorEvent.MinDistance;
                    maxDistance.floatValue = editorEvent.MaxDistance;
                }
                EditorGUI.BeginDisabledGroup(!overrideAtt.boolValue);
                EditorGUIUtility.labelWidth = 30;
                minDistance.floatValue = EditorGUILayout.FloatField("Min", minDistance.floatValue);
                minDistance.floatValue = Mathf.Clamp(minDistance.floatValue, 0, maxDistance.floatValue);
                maxDistance.floatValue = EditorGUILayout.FloatField("Max", maxDistance.floatValue);
                maxDistance.floatValue = Mathf.Max(minDistance.floatValue, maxDistance.floatValue);
                EditorGUIUtility.labelWidth = 0;
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                */
            }

            param.isExpanded = EditorGUILayout.Foldout(param.isExpanded, "Initial Parameter Values");
            if (ev.hasMultipleDifferentValues)
            {
                if (param.isExpanded)
                {
                    GUILayout.Box("Cannot change parameters when different events are selected", GUILayout.ExpandWidth(true));
                }
            }
            else
            {
                var eventRef = FMODUnity.EventManager.EventFromPath(ev.stringValue);
                if (param.isExpanded && eventRef != null)
                {
                    foreach (var paramRef in eventRef.Parameters)
                    {
                        bool set;
                        float value;
                        bool matchingSet, matchingValue;
                        CheckParameter(paramRef.Name, out set, out matchingSet, out value, out matchingValue);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(paramRef.Name);
                        EditorGUI.showMixedValue = !matchingSet;
                        EditorGUI.BeginChangeCheck();
                        bool newSet = EditorGUILayout.Toggle(set, GUILayout.Width(20));
                        EditorGUI.showMixedValue = false;

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObjects(serializedObject.isEditingMultipleObjects ? serializedObject.targetObjects : new UnityEngine.Object[] { serializedObject.targetObject }, "Inspector");
                            if (newSet)
                            {
                                AddParameterValue(paramRef.Name, paramRef.Default);
                            }
                            else
                            {
                                DeleteParameterValue(paramRef.Name);
                            }
                            set = newSet;
                        }

                        EditorGUI.BeginDisabledGroup(!newSet);
                        if (set)
                        {
                            EditorGUI.showMixedValue = !matchingValue;
                            EditorGUI.BeginChangeCheck();
                            value = EditorGUILayout.Slider(value, paramRef.Min, paramRef.Max);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObjects(serializedObject.isEditingMultipleObjects ? serializedObject.targetObjects : new UnityEngine.Object[] { serializedObject.targetObject }, "Inspector");
                                SetParameterValue(paramRef.Name, value);
                            }
                            EditorGUI.showMixedValue = false;
                        }
                        else
                        {
                            EditorGUI.showMixedValue = !matchingValue;
                            EditorGUILayout.Slider(paramRef.Default, paramRef.Min, paramRef.Max);
                            EditorGUI.showMixedValue = false;
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }

                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void CheckParameter(string name, out bool set, out bool matchingSet, out float value, out bool matchingValue)
    {
        value = 0;
        set = false;
        if (serializedObject.isEditingMultipleObjects)
        {
            bool first = true;
            matchingValue = true;
            matchingSet = true;
            foreach (var obj in serializedObject.targetObjects)
            {
                var emitter = obj as FMODEmitter;
                var param = emitter.parameters != null ? emitter.parameters.FirstOrDefault((x) => x.Name == name) : null;
                if (first)
                {
                    set = param != null;
                    value = set ? param.Value : 0;
                    first = false;
                }
                else
                {
                    if (set)
                    {
                        if (param == null)
                        {
                            matchingSet = false;
                            matchingValue = false;
                            return;
                        }
                        else
                        {
                            if (param.Value != value)
                            {
                                matchingValue = false;
                            }
                        }
                    }
                    else
                    {
                        if (param != null)
                        {
                            matchingSet = false;
                        }
                    }
                }
            }
        }
        else
        {
            matchingSet = matchingValue = true;

            var emitter = serializedObject.targetObject as FMODEmitter;
            var param = emitter.parameters != null ? emitter.parameters.FirstOrDefault((x) => x.Name == name) : null;
            if (param != null)
            {
                set = true;
                value = param.Value;
            }
        }
    }

    void SetParameterValue(string name, float value)
    {            
        if (serializedObject.isEditingMultipleObjects)
        {
            foreach (var obj in serializedObject.targetObjects)
            {
                SetParameterValue(obj, name, value);
            }
        }
        else
        {
            SetParameterValue(serializedObject.targetObject, name, value);
        }
    }

    void SetParameterValue(UnityEngine.Object obj, string name, float value)
    {
        var emitter = obj as FMODEmitter;
        var param = emitter.parameters != null ? emitter.parameters.FirstOrDefault((x) => x.Name == name) : null;
        if (param != null)
        {
            param.Value = value;
        }
    }


    void AddParameterValue(string name, float value)
    {
        if (serializedObject.isEditingMultipleObjects)
        {
            foreach (var obj in serializedObject.targetObjects)
            {
                AddParameterValue(obj, name, value);
            }
        }
        else
        {
            AddParameterValue(serializedObject.targetObject, name, value);
        }
    }

    void AddParameterValue(UnityEngine.Object obj, string name, float value)
    {
        var emitter = obj as FMODEmitter;
        var param = emitter.parameters != null ? emitter.parameters.FirstOrDefault((x) => x.Name == name) : null;
        if (param == null)
        {
            int end = emitter.parameters.Length;
            Array.Resize<FMODUnity.ParamRef>(ref emitter.parameters, end + 1);
            emitter.parameters[end] = new FMODUnity.ParamRef
            {
                Name = name,
                Value = value
            };
        }
    }

    void DeleteParameterValue(string name)
    {
        if (serializedObject.isEditingMultipleObjects)
        {
            foreach (var obj in serializedObject.targetObjects)
            {
                DeleteParameterValue(obj, name);
            }
        }
        else
        {
            DeleteParameterValue(serializedObject.targetObject, name);
        }
    }

    void DeleteParameterValue(UnityEngine.Object obj, string name)
    {
        var emitter = obj as FMODEmitter;
        int found = -1;
        for (int i = 0; i < emitter.parameters.Length; i++)
        {
            if (emitter.parameters[i].Name == name)
            {
                found = i;
            }
        }
        if (found >= 0)
        {
            int end = emitter.parameters.Length - 1;
            emitter.parameters[found] = emitter.parameters[end];
            Array.Resize<FMODUnity.ParamRef>(ref emitter.parameters, end);
        }
    }
}