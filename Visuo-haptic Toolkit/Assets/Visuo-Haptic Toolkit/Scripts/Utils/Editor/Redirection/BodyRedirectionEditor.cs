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
		SerializedProperty physicalHand;
		SerializedProperty virtualHand;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty targetsScene;
		SerializedProperty radius;
		SerializedProperty applyDampening;
		SerializedProperty applySmoothing;

		private void OnEnable() {
			technique = serializedObject.FindProperty("technique");
            techniqueInstance = serializedObject.FindProperty("techniqueInstance");

			scene = serializedObject.FindProperty("scene");
			physicalHand = serializedObject.FindProperty("scene.physicalHand");
			virtualHand = serializedObject.FindProperty("scene.virtualHand");
			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			targetsScene = serializedObject.FindProperty("scene.targets");
			radius = serializedObject.FindProperty("scene.radius");
			applyDampening = serializedObject.FindProperty("scene.applyDampening");
			applySmoothing = serializedObject.FindProperty("scene.applySmoothing");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				EditorGUILayout.PropertyField(techniqueInstance, new GUIContent ("Parameters"));
			}

			// Scene
			// EditorGUILayout.PropertyField(scene, new GUIContent("Scene"));
			EditorGUILayout.PropertyField(physicalHand, new GUIContent("Physical Hand"));
			EditorGUILayout.PropertyField(physicalHand, new GUIContent("Virtual Hand"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}