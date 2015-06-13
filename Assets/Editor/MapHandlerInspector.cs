using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MapHandler))]
public class MapHandlerInspector : Editor {

    private static GUIContent generateButtonText = new GUIContent("Generate Map!");

    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        MapHandler handler = target as MapHandler;
        DrawDefaultInspector();
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
