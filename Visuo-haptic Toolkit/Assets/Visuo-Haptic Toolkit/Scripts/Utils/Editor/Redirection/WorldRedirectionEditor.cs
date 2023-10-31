using System;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(WorldRedirection))]
	public class WorldRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty strategy;

		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty targetsScene;
		SerializedProperty radius;
		SerializedProperty applyDampening;
		SerializedProperty applySmoothing;

        readonly string[] strategyTechniques = { "Razzaque2001OverTimeRotation", "Razzaque2001Rotational", "Razzaque2001Curvature", "Razzaque2001Hybrid" };

		private void OnEnable() {
			technique = serializedObject.FindProperty("technique");
			strategy = serializedObject.FindProperty("strategy");

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
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((WorldRedirection)target), typeof(WorldRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));
			if (strategyTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(strategy, new GUIContent ("Strategy"));
			}

			// Scene
			EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
			EditorGUILayout.PropertyField(virtualHead, new GUIContent("Virtual Head"));

			EditorGUILayout.PropertyField(physicalTarget, new GUIContent("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent("Virtual Target"));
			EditorGUILayout.PropertyField(origin, new GUIContent("Origin"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001Hybrid") {
				// if (technique.enumNames[technique.enumValueIndex] == ) {

				EditorGUILayout.PropertyField(targetsScene, new GUIContent ("Targets"));
				EditorGUILayout.PropertyField(radius, new GUIContent ("Radius"));
				EditorGUILayout.PropertyField(applyDampening, new GUIContent ("Apply Dampening"));
				EditorGUILayout.PropertyField(applySmoothing, new GUIContent ("Apply Smoothing"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}