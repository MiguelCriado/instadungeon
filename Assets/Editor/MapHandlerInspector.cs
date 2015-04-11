using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MapHandler))]
public class MapHandlerInspector : Editor {
    private static GUIContent generateButtonText = new GUIContent("Generate Map!");

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        MapHandler handler = target as MapHandler;
        if (Application.isPlaying)
        {
            if (GUILayout.Button(generateButtonText))
            {
                handler.Generate();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
