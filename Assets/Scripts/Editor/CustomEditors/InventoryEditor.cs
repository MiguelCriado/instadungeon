using InstaDungeon.Components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InstaDungeon.Editors
{

	// IDEAS:
	// - Setup null inventory fields by dragging objects to a drop area. So cool!

	//[CustomEditor(typeof(Inventory))]
	//public class InventoryEditor : Editor
	//{
	//	public override void OnInspectorGUI()
	//	{
	//		serializedObject.Update();

	//		SerializedProperty keys = serializedObject.FindProperty("keys");
	//		SerializedProperty values = serializedObject.FindProperty("values");

	//		EditorGUILayout.LabelField("Equipment", EditorStyles.boldLabel);

	//		EditorGUILayout.BeginHorizontal("box");
	//		EditorGUILayout.BeginVertical();
	//		{

	//			EditorGUILayout.LabelField("Head");

	//		}
	//		EditorGUILayout.EndVertical();
	//		EditorGUILayout.EndHorizontal();

	//		serializedObject.ApplyModifiedProperties();
	//	}
	//}
}
