using System;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection.WorldRedirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(WorldRedirection))]
	public class WorldRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty strategy;

		SerializedProperty physicalHand;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty targetsScene;
		SerializedProperty radius;
		SerializedProperty applyDampening;
		SerializedProperty applySmoothing;
		SerializedProperty redirect;

        readonly string[] strategyTechniques = { "Razzaque2001OverTimeRotation", "Razzaque2001Rotational", "Razzaque2001Curvature", "Razzaque2001Hybrid" };
        readonly string[] targetsStrategies = { "SteerToCenter", "SteerToMultipleTargets", "SteerInDirection" };

		private void OnEnable() {
			technique = serializedObject.FindProperty("technique");
			strategy = serializedObject.FindProperty("strategy");

			// Scene
			physicalHand = serializedObject.FindProperty("scene.limbs");

			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			targetsScene = serializedObject.FindProperty("scene.targets");
			radius = serializedObject.FindProperty("scene.radius");
			applyDampening = serializedObject.FindProperty("scene.applyDampening");
			applySmoothing = serializedObject.FindProperty("scene.applySmoothing");
			redirect = serializedObject.FindProperty("redirect");
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

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(physicalHand, new GUIContent("Physical Hand"));
			EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
			EditorGUILayout.PropertyField(virtualHead, new GUIContent("Virtual Head"));

			if (technique.enumNames[technique.enumValueIndex] == "Azmandian2016World") {
				EditorGUILayout.PropertyField(physicalTarget, new GUIContent("Physical Target"));
				EditorGUILayout.PropertyField(virtualTarget, new GUIContent("Virtual Target"));
				EditorGUILayout.PropertyField(origin, new GUIContent("Origin"));
			}


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(redirect, new GUIContent("Redirect"));

			// Hides targets, dampening and smoothing if
			if (strategyTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				if (targetsStrategies.Contains(strategy.enumNames[strategy.enumValueIndex])) {
					EditorGUILayout.PropertyField(targetsScene, new GUIContent ("Targets"));
				} else if (strategy.enumNames[strategy.enumValueIndex] == "SteerToOrbit") {
					EditorGUILayout.PropertyField(radius, new GUIContent ("Radius"));
				}

				EditorGUILayout.PropertyField(applyDampening, new GUIContent ("Apply Dampening"));
				EditorGUILayout.PropertyField(applySmoothing, new GUIContent ("Apply Smoothing"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}