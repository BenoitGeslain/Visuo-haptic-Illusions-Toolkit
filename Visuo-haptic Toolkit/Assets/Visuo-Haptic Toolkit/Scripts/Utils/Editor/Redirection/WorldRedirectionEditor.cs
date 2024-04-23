using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace VHToolkit.Redirection.WorldRedirection {
	/// <summary>
	/// Custom editor for the WorldRedirection Monobehaviour.
	/// </summary>
	[CustomEditor(typeof(WorldRedirection))]
	public class WorldRedirectionEditor : Editor {
		SerializedProperty technique, strategy,
		limbs, physicalHead, virtualHead,
		physicalTarget, virtualTarget,
		origin,
		targetsScene,
		radius,
		applyDampening, applySmoothing,
		redirect, direction,
		enableOverTime, enableRotational, enableCurvature,
		parameters;
		SerializedObject parametersObject;
		readonly HashSet<string> strategyTechniques = new() { "Razzaque2001OverTimeRotation", "Razzaque2001Rotational", "Razzaque2001Curvature", "Razzaque2001Hybrid" };
		readonly HashSet<string> targetsStrategies = new() { "SteerToCenter", "SteerToMultipleTargets" };

		private void OnEnable() {
			technique = serializedObject.FindProperty("_technique");
			strategy = serializedObject.FindProperty("strategy");

			// Scene
			limbs = serializedObject.FindProperty("scene.limbs");

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
			enableOverTime = serializedObject.FindProperty("scene.enableHybridOverTime");
			enableRotational = serializedObject.FindProperty("scene.enableHybridRotational");
			enableCurvature = serializedObject.FindProperty("scene.enableHybridCurvature");

			parameters = serializedObject.FindProperty("scene.parameters");
		}

		private void MakePropertyField(SerializedProperty property, string text, string tooltip = null) {
			EditorGUILayout.PropertyField(property, new GUIContent(text, tooltip));
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((WorldRedirection)target), typeof(WorldRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.largeLabel);

			string techniqueName = technique.enumNames[technique.enumValueIndex];

			MakePropertyField(limbs, "User Limbs", "A list of tracked user limbs.");
			MakePropertyField(physicalHead, "Physical Head", "Transform tracking the user's real head");
			MakePropertyField(virtualHead, "Virtual Head", "Transform tracking the user's virtual head");

			if (techniqueName == nameof(Azmandian2016World)) {
				MakePropertyField(physicalTarget, "Physical Target");
				MakePropertyField(virtualTarget, "Virtual Target");
				MakePropertyField(origin, "Origin");
			}


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.largeLabel);

			MakePropertyField(technique, "Redirection technique");

			MakePropertyField(redirect, "Activate Redirection");
			MakePropertyField(parameters, "Numerical Parameters");

			// If no parameters Scriptable object, update object and don't render the rest of the view
			if (parameters.objectReferenceValue == null) {
				serializedObject.ApplyModifiedProperties();
				return;
			}

			parametersObject = new SerializedObject(parameters.objectReferenceValue);
			parametersObject.Update();

			EditorGUILayout.Space(2);
			if (techniqueName == nameof(Razzaque2001OverTimeRotation)) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("OverTimeRotation"), "Over Time Rotation Rate");
			} else if (techniqueName == nameof(Razzaque2001Rotational)) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("GainsRotational"), "Rotational Gains");
				MakePropertyField(parametersObject.FindProperty("RotationalThreshold"), "Rotational Threshold");
			} else if (techniqueName == nameof(Razzaque2001Curvature)) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("CurvatureRadius"), "Curvature Radius");
				MakePropertyField(parametersObject.FindProperty("WalkingThreshold"), "Walking Threshold");
			} else if (techniqueName == nameof(Razzaque2001Hybrid)) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(enableOverTime, "Enable Over Time Rotation");
				if (enableOverTime.boolValue)
					MakePropertyField(parametersObject.FindProperty("OverTimeRotation"), "Over Time Rotation Rate");
				EditorGUILayout.Space(2);
				MakePropertyField(serializedObject.FindProperty("scene.enableHybridRotational"), "Enable Rotational");
				if (enableRotational.boolValue) {
					MakePropertyField(parametersObject.FindProperty("GainsRotational"), "Rotational Gains");
					MakePropertyField(parametersObject.FindProperty("RotationalThreshold"), "Rotational Threshold");
				}
				EditorGUILayout.Space(2);
				MakePropertyField(serializedObject.FindProperty("scene.enableHybridCurvature"), "Enable Curvature");
				if (enableCurvature.boolValue) {
					MakePropertyField(parametersObject.FindProperty("CurvatureRadius"), "Curvature Radius");
					MakePropertyField(parametersObject.FindProperty("WalkingThreshold"), "Walking Threshold");
				}
				EditorGUILayout.Space(2);
				MakePropertyField(serializedObject.FindProperty("scene.aggregateFunction"), "Aggregate Function");

				WorldRedirection script = target as WorldRedirection;
				switch (script.scene.aggregateFunction) {
					case HybridAggregate.Max:
						script.techniqueInstance = Razzaque2001Hybrid.Max();
						break;
					case HybridAggregate.Sum:
						script.techniqueInstance = Razzaque2001Hybrid.Sum();
						break;
					case HybridAggregate.Mean:
						script.techniqueInstance = Razzaque2001Hybrid.Mean();
						break;
					case HybridAggregate.WeightedSum:
						MakePropertyField(parametersObject.FindProperty("HybridWeights"), "Hybrid Weights");
						Vector3 w = script.scene.parameters.HybridWeights;
						script.techniqueInstance = Razzaque2001Hybrid.Weighted(w.x, w.y, w.z);
						break;
					default:
						script.techniqueInstance = Razzaque2001Hybrid.Max();
						break;
				}
			} else if (techniqueName == nameof(Azmandian2016World)) {
				MakePropertyField(parametersObject.FindProperty("GainsRotational"), "Gains Rotational");
			} else if (techniqueName == nameof(Steinicke2008Translational)) {
				MakePropertyField(parametersObject.FindProperty("GainsTranslational"), "Translational Gains");
			}


			// Hides targets, dampening and smoothing if
			if (strategyTechniques.Contains(techniqueName)) {
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.largeLabel);
				MakePropertyField(strategy, "Target selection strategy");

				if (targetsStrategies.Contains(strategy.enumNames[strategy.enumValueIndex])) {
					MakePropertyField(targetsScene, "Targets");
					MakePropertyField(applyDampening, "Apply Dampening");
					MakePropertyField(parametersObject.FindProperty("DampeningDistanceThreshold"), "Dampening Distance Threshold");
					MakePropertyField(parametersObject.FindProperty("DampeningRange"), "Dampening Range");
					MakePropertyField(applySmoothing, "Apply Smoothing");
					MakePropertyField(parametersObject.FindProperty("SmoothingFactor"), "Smoothing Factor");
				} else if (strategy.enumNames[strategy.enumValueIndex] == nameof(SteerToOrbit)) {
					MakePropertyField(targetsScene, "Targets");

					var steerToOrbitRadius = parametersObject.FindProperty("SteerToOrbitRadius");
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(steerToOrbitRadius, new GUIContent("Steer To Orbit Radius"));
						EditorGUILayout.Space(20);
						EditorGUILayout.LabelField("Rotation Rate: " + (360f / (2 * Mathf.PI * steerToOrbitRadius.floatValue)).ToString("N2") + " °/m/s");
					GUILayout.EndHorizontal();

				} else if (strategy.enumNames[strategy.enumValueIndex] == nameof(SteerInDirection)) {
					MakePropertyField(direction, "Direction");
				}
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}