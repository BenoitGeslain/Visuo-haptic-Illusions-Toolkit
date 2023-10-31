using System;

using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalHand;
		SerializedProperty virtualHand;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;

		private void OnEnable() {

			technique = serializedObject.FindProperty("technique");
            techniqueInstance = serializedObject.FindProperty("techniqueInstance");

			physicalHand = serializedObject.FindProperty("scene.physicalHand");
			virtualHand = serializedObject.FindProperty("scene.virtualHand");
			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BodyRedirection)target), typeof(BodyRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));

			// Scene
			EditorGUILayout.PropertyField(physicalHand, new GUIContent("Physical Hand"));
			EditorGUILayout.PropertyField(virtualHand, new GUIContent("Virtual Hand"));
			if (technique.enumNames[technique.enumValueIndex] == "Azmandian2016Hybrid") {
				EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
				EditorGUILayout.PropertyField(virtualHead, new GUIContent("Virtual Head"));
			}

			EditorGUILayout.PropertyField(physicalTarget, new GUIContent("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent("Virtual Target"));
			EditorGUILayout.PropertyField(origin, new GUIContent("Origin"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				EditorGUILayout.PropertyField(techniqueInstance, new GUIContent ("Parameters"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}