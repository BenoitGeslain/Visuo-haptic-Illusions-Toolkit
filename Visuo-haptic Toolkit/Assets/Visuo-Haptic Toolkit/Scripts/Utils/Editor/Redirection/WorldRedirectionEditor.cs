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
		SerializedProperty direction;
		SerializedProperty parameters;
		SerializedObject parametersObject;

		readonly string[] strategyTechniques = { "Razzaque2001OverTimeRotation", "Razzaque2001Rotational", "Razzaque2001Curvature", "Razzaque2001Hybrid" };
        readonly string[] targetsStrategies = { "SteerToCenter", "SteerToMultipleTargets" };

		private void OnEnable() {
			technique = serializedObject.FindProperty("_technique");
			strategy = serializedObject.FindProperty("strategy");

			// Scene
			physicalHand = serializedObject.FindProperty("scene.limbs");

			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			targetsScene = serializedObject.FindProperty("scene.targets");
			applyDampening = serializedObject.FindProperty("scene.applyDampening");
			applySmoothing = serializedObject.FindProperty("scene.applySmoothing");
			redirect = serializedObject.FindProperty("redirect");
			direction = serializedObject.FindProperty("scene.strategyDirection");

			parameters = serializedObject.FindProperty("scene.parameters");
			parametersObject = new SerializedObject(parameters.objectReferenceValue);
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((WorldRedirection)target), typeof(WorldRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();
			parametersObject.Update();

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

			EditorGUILayout.PropertyField(parameters, new GUIContent("Parameters"));

			// WorldRedirection someComponent = target as WorldRedirection;
			// // if (someComponent == null)
			// // 	someComponent = CreateInstance<ParametersToolkit>();
			// Debug.Log(someComponent.scene);
			// Debug.Log(someComponent.scene.parameters);
			// Debug.Log(someComponent.scene.parameters.OverTimeRotation);

			if (new string[] {"Razzaque2001OverTimeRotation", "Razzaque2001Hybrid"}.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("OverTimeRotation"), new GUIContent("Over Time Rotation Rate"));
			}
			if (new string[] { "Razzaque2001Rotational", "Razzaque2001Hybrid" }.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsRotational"), new GUIContent("Rotational Gains"));
			}
			if (new string[] { "Razzaque2001Curvature", "Razzaque2001Hybrid" }.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("CurvatureRadius"), new GUIContent("Curvature Radius"));
			}
			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001Hybrid") {
				// TODO: Radio list to choose which techniques to activate
			}
			// if (technique.enumNames[technique.enumValueIndex] == "Azmandian2016World") {
			// 	EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsRotational"), new GUIContent("Gains Rotational"));
			// }
			if (technique.enumNames[technique.enumValueIndex] == "Steinicke2008Translational") {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsTranslational"), new GUIContent("Translational Gains"));
			}

			EditorGUILayout.PropertyField(redirect, new GUIContent("Redirect"));

			// Hides targets, dampening and smoothing if
			if (strategyTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				if (targetsStrategies.Contains(strategy.enumNames[strategy.enumValueIndex])) {
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.boldLabel);
					EditorGUILayout.PropertyField(targetsScene, new GUIContent ("Targets"));
					EditorGUILayout.PropertyField(applyDampening, new GUIContent("Apply Dampening"));
					EditorGUILayout.PropertyField(applySmoothing, new GUIContent("Apply Smoothing"));
				} else if (strategy.enumNames[strategy.enumValueIndex] == "SteerInDirection") {
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.boldLabel);
					EditorGUILayout.PropertyField(direction, new GUIContent ("Direction"));
				}

			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}