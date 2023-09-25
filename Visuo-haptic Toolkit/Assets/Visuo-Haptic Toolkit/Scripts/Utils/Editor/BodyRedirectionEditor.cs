using System;

using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty scene;

		private void OnEnable() {
			technique = serializedObject.FindProperty("technique");
            techniqueInstance = serializedObject.FindProperty("techniqueInstance");
			scene = serializedObject.FindProperty("scene");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				EditorGUILayout.PropertyField(techniqueInstance, new GUIContent ("Parameters"));
			}
			EditorGUILayout.PropertyField(scene, new GUIContent("Scene"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}