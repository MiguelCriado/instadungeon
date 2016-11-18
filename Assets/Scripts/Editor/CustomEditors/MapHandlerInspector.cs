using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MapGenerator))]
public class MapHandlerInspector : Editor
{
    private static GUIContent generateButtonText = new GUIContent("Generate Map!");
    private static string ONLY_ONE_LAYOUT_GENERATOR = "There should only be one (1) Layout Generator attached to this object.";
    private static string ONLY_ONE_SHAPE_GENERATOR = "There should only be one (1) Shape Generator attached to this object.";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MapGenerator handler = target as MapGenerator;
        CheckGeneratorsPresence(handler);

        // Generators
        EditorGUILayout.PropertyField(serializedObject.FindProperty("layoutType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shapeType"));

        // RandomSeed row
        GUILayout.BeginHorizontal();
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("customSeed"), true);

			if (!handler.customSeed)
			{
				GUI.enabled = false;
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("levelSeed"), GUIContent.none);
			GUI.enabled = true;
		}
        GUILayout.EndHorizontal();

        // Generate Dungeon button
        if (Application.isPlaying)
        {
            if (GUILayout.Button(generateButtonText))
            {
				handler.GenerateNewMap();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CheckGeneratorsPresence(MapGenerator handler)
    {
        ILayoutGenerator[] layoutGenerators = handler.gameObject.GetComponents<ILayoutGenerator>();

        if (layoutGenerators.Length < 1)
        {
            AddLayoutGenerator(handler);
        }
		else if (layoutGenerators.Length > 1)
        {
            EditorGUILayout.HelpBox(ONLY_ONE_LAYOUT_GENERATOR, MessageType.Warning, true);
        }

        IZoneGenerator[] shapeGenerators = handler.gameObject.GetComponents<IZoneGenerator>();

        if (shapeGenerators.Length < 1)
        {
            AddShapeGenerator(handler);
        }
		else if (shapeGenerators.Length > 1)
        {
            EditorGUILayout.HelpBox(ONLY_ONE_SHAPE_GENERATOR, MessageType.Warning, true);
        }
    }

    private void AddLayoutGenerator(MapGenerator handler)
    {
        switch (handler.layoutType)
        {
            case MapGenerator.LayoutType.Hilbert:
                handler.gameObject.AddComponent<HilbertLayoutGenerator>();
                break;
        }
    }

    private void AddShapeGenerator(MapGenerator handler)
    {
        switch (handler.shapeType)
        {
            case MapGenerator.ShapeType.Cavernous:
                handler.gameObject.AddComponent<CavernousZoneGenerator>();
                break;
        }
    }
}
