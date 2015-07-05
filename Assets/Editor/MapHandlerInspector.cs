using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(MapHandler))]
public class MapHandlerInspector : Editor {

    private static GUIContent generateButtonText = new GUIContent("Generate Map!");
    private static GUIContent tilePrefabFoldoutText = new GUIContent("Custom blocks");
    private static string ONLY_ONE_LAYOUT_GENERATOR = "There should only be one (1) Layout Generator attached to this object.";
    private static string ONLY_ONE_SHAPE_GENERATOR = "There should only be one (1) Shape Generator attached to this object.";

    private bool showTilePrefabs = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MapHandler handler = target as MapHandler;
        CheckGeneratorsPresence(handler);
        // DungeonType
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonType"));

        // Generators
        EditorGUILayout.PropertyField(serializedObject.FindProperty("layoutType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeType"));

        // RandomSeed row
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("customSeed"), true);
        if (!handler.customSeed) GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelSeed"),GUIContent.none);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        // Tile Prefabs
        showTilePrefabs = EditorGUILayout.Foldout(showTilePrefabs, tilePrefabFoldoutText);
        if (showTilePrefabs)
        {
            switch (handler.dungeonType)
            {
                case MapHandler.DungeonType._3D:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("floorPrefab_3D"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallPrefab_3D"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("entranceStairs_3D"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("exitStairs_3D"));
                    break;
                case MapHandler.DungeonType.Isometric:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("floorPrefab_iso"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallPrefab_iso"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("entranceStairs_iso"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("exitStairs_iso"));
                    break;
                case MapHandler.DungeonType.Orthogonal:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("floorPrefab_ortho"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("wallPrefab_ortho"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("entranceStairs_ortho"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("exitStairs_ortho"));
                    break;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
        }

        // Generate Dungeon button
        if (Application.isPlaying)
        {
            if (GUILayout.Button(generateButtonText))
            {
                handler.Generate();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void CheckGeneratorsPresence(MapHandler handler)
    {
        LayoutGenerator[] layoutGenerators = handler.gameObject.GetComponents<LayoutGenerator>();
        if (layoutGenerators.Length < 1)
        {
            AddLayoutGenerator(handler);
        } else if (layoutGenerators.Length > 1)
        {
            EditorGUILayout.HelpBox(ONLY_ONE_LAYOUT_GENERATOR, MessageType.Warning, true);
        }
        ShapeGenerator[] shapeGenerators = handler.gameObject.GetComponents<ShapeGenerator>();
        if (shapeGenerators.Length < 1)
        {
            AddShapeGenerator(handler);
        } else if (shapeGenerators.Length > 1)
        {
            EditorGUILayout.HelpBox(ONLY_ONE_SHAPE_GENERATOR, MessageType.Warning, true);
        }
    }

    private void AddLayoutGenerator(MapHandler handler)
    {
        switch (handler.layoutType)
        {
            case MapHandler.LayoutType.Hilbert:
                handler.gameObject.AddComponent<HilbertLayoutGenerator>();
                break;
        }
    }

    private void AddShapeGenerator(MapHandler handler)
    {
        switch (handler.shapeType)
        {
            case MapHandler.ShapeType.Cavernous:
                handler.gameObject.AddComponent<CavernousShapeGenerator>();
                break;
        }
    }
}
