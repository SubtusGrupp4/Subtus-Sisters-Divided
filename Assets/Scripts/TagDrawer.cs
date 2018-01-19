using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GiveTagAttribute))]
public class TagDrawer : PropertyDrawer
{
    // Overide GUI, a must incase you wanna change the GUI ...
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Is it a Seralized String? ( in simple, is it a public string? )
        if (property.propertyType == SerializedPropertyType.String)
        {
            //Begin Change
            EditorGUI.BeginProperty(position, label, property);

            // using the existing TagField, which creates a window of the existing tags. 
            // and then simply change the value to the selected String (Tag).
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);

            // End Change
            EditorGUI.EndProperty();
        }
        else // Incase it's not a seralized String use standard "drawing".
            EditorGUI.PropertyField(position, property, label);
    }
}