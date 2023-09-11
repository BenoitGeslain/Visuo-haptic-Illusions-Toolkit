using System;

using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	/// <summary>
	/// Custom editor for the body redirection scene.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalHand;
		SerializedProperty virtualHand;
		SerializedProperty origin;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;

		private void OnEnable() {
			technique = serializedObject.FindProperty("technique");
			techniqueInstance = serializedObject.FindProperty ("techniqueInstance");

			physicalHand = serializedObject.FindProperty("physicalHand");
			virtualHand = serializedObject.FindProperty("virtualHand");
			origin = serializedObject.FindProperty("origin");
			physicalTarget = serializedObject.FindProperty("physicalTarget");
			virtualTarget = serializedObject.FindProperty("virtualTarget");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));

			// Hides a2 and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				EditorGUILayout.PropertyField(techniqueInstance, new GUIContent ("Parameters"));
			}

			EditorGUILayout.PropertyField(physicalHand, new GUIContent ("Physical Hand"));
			EditorGUILayout.PropertyField(virtualHand, new GUIContent ("Virtual Hand"));
			EditorGUILayout.PropertyField(origin, new GUIContent ("Origin"));
			EditorGUILayout.PropertyField(physicalTarget, new GUIContent ("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent ("Virtual Target"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}