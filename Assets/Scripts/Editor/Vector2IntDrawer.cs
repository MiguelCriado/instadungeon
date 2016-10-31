using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Vector2Int))]
public class Vector2IntDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        contentPosition.width *= 0.5f;
        EditorGUIUtility.labelWidth = 14f;
        GUI.enabled = false;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("_x"));
        contentPosition.x += contentPosition.width;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("_y"));
        GUI.enabled = true;
        EditorGUI.EndProperty();
    }
}
