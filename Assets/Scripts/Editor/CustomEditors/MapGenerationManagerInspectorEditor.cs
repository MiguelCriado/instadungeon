using UnityEditor;
using UnityEngine;

namespace InstaDungeon.Editors
{
	[CustomEditor(typeof(MapGenerationManagerInspector))]
	public class MapGenerationManagerInspectorEditor : Editor
	{
		private MapGenerationManagerInspector inspector;

		private void OnEnable()
		{
			inspector = target as MapGenerationManagerInspector;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("customSeed"), new GUIContent("Custom Seed"));

			GUI.enabled = inspector.CustomSeed;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("levelSeed"), new GUIContent("Level Seed"));
			GUI.enabled = true;

			serializedObject.ApplyModifiedProperties();
		}
	}
}
