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

			EditorGUILayout.PropertyField(technique, new GUIContent("Technique"));
			if (strategyTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				EditorGUILayout.PropertyField(strategy, new GUIContent("Strategy"));
			}

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(physicalHand, new GUIContent("Physical Limbs"));
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
			EditorGUILayout.PropertyField(parameters, new GUIContent("Parameters"));

			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001OverTimeRotation") {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("OverTimeRotation"), new GUIContent("Over Time Rotation Rate"));
			}
			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001Rotational") {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsRotational"), new GUIContent("Rotational Gains"));
			}
			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001Curvature") {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("CurvatureRadius"), new GUIContent("Curvature Radius"));
			}
			if (technique.enumNames[technique.enumValueIndex] == "Razzaque2001Hybrid") {
				EditorGUILayout.PropertyField(serializedObject.FindProperty("scene.enableHybridOverTime"), new GUIContent("Enable Over Time Rotation"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("OverTimeRotation"), new GUIContent("Over Time Rotation Rate"));
				EditorGUILayout.Space(2);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("scene.enableHybridRotational"), new GUIContent("Enable Rotational"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsRotational"), new GUIContent("Rotational Gains"));
				EditorGUILayout.Space(2);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("scene.enableHybridCurvature"), new GUIContent("Enable Curvature"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("CurvatureRadius"), new GUIContent("Curvature Radius"));
				EditorGUILayout.Space(2);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("scene.aggregateFunction"), new GUIContent("Aggregate Function"));

				WorldRedirection script = target as WorldRedirection;
				switch (script.scene.aggregateFunction) {
					case HybridAggregate.Max:
						script.techniqueInstance = Razzaque2001Hybrid.Max();
						break;
					case HybridAggregate.Sum:
						script.techniqueInstance = Razzaque2001Hybrid.Sum();
						break;
					case HybridAggregate.WeightedSum:
						EditorGUILayout.PropertyField(parametersObject.FindProperty("HybridWeights"), new GUIContent("HybridWeights"));
						Vector3 w = script.scene.parameters.HybridWeights;
						script.techniqueInstance = Razzaque2001Hybrid.Weighted(w.x, w.y, w.z);
						break;
					default:
						script.techniqueInstance = Razzaque2001Hybrid.Max();
						break;
				}
			}
			// if (technique.enumNames[technique.enumValueIndex] == "Azmandian2016World") {
			// 	EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsRotational"), new GUIContent("Gains Rotational"));
			// }
			if (technique.enumNames[technique.enumValueIndex] == "Steinicke2008Translational") {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GainsTranslational"), new GUIContent("Translational Gains"));
			}

			// Hides targets, dampening and smoothing if
			if (strategyTechniques.Contains(technique.enumNames[technique.enumValueIndex])) {
				if (targetsStrategies.Contains(strategy.enumNames[strategy.enumValueIndex])) {
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.boldLabel);
					EditorGUILayout.PropertyField(targetsScene, new GUIContent("Targets"));
					EditorGUILayout.PropertyField(applyDampening, new GUIContent("Apply Dampening"));
					EditorGUILayout.PropertyField(applySmoothing, new GUIContent("Apply Smoothing"));
				}
				else if (strategy.enumNames[strategy.enumValueIndex] == "SteerToOrbit") {
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.boldLabel);
					var steerToOrbitRadius = parametersObject.FindProperty("steerToOrbitRadius");
					EditorGUILayout.PropertyField(steerToOrbitRadius, new GUIContent("Orbit Radius"));
				}
				else if (strategy.enumNames[strategy.enumValueIndex] == "SteerInDirection") {
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.boldLabel);
					EditorGUILayout.PropertyField(direction, new GUIContent("Direction"));
				}

			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}