using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System;
using System.Linq;

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
		bounds,
		parameters;
		SerializedObject parametersObject;

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
			bounds = serializedObject.FindProperty("scene.bounds");

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

			Enum.TryParse(technique.enumNames[technique.enumValueIndex], out WRTechnique actualTechnique);
			var enumType = typeof(WRTechnique);
			var memberInfos = enumType.GetMember(actualTechnique.ToString());
			var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);

			MakePropertyField(limbs, "User Limbs", "A list of tracked user limbs.");
			MakePropertyField(physicalHead, "Physical Head", "Transform tracking the user's real head");
			MakePropertyField(virtualHead, "Virtual Head", "Transform tracking the user's virtual head");

			if (actualTechnique == WRTechnique.Azmandian2016World) {
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
			if (actualTechnique == WRTechnique.Razzaque2001OverTimeRotation) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("OverTimeRotation"), "Over Time Rotation Rate");
			}
			else if (actualTechnique == WRTechnique.Razzaque2001Rotational) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("GainsRotational"), "Rotational Gains");
				MakePropertyField(parametersObject.FindProperty("RotationalThreshold"), "Rotational Threshold");
			}
			else if (actualTechnique == WRTechnique.Razzaque2001Curvature) {
				MakePropertyField(parametersObject.FindProperty("RotationalError"), "Rotational Error");
				MakePropertyField(parametersObject.FindProperty("CurvatureRadius"), "Curvature Radius");
				MakePropertyField(parametersObject.FindProperty("WalkingThreshold"), "Walking Threshold");
			}
			else if (actualTechnique == WRTechnique.Razzaque2001Hybrid) {
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
			}
			else if (actualTechnique == WRTechnique.Azmandian2016World) {
				MakePropertyField(parametersObject.FindProperty("GainsRotational"), "Gains Rotational");
			}
			else if (actualTechnique == WRTechnique.Steinicke2008Translational) {
				MakePropertyField(parametersObject.FindProperty("GainsTranslational"), "Translational Gains");
			}


			// Hides targets, dampening and smoothing if
			if (enumValueMemberInfo.IsDefined(typeof(HasStrategyAttribute), false)) {
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Strategy Parameters", EditorStyles.largeLabel);
				MakePropertyField(strategy, "Target selection strategy");

				Enum.TryParse(strategy.enumNames[strategy.enumValueIndex], out WRStrategy actualStrategy);
				var strategyEnumType = typeof(WRStrategy);
				var strategyMemberInfos = strategyEnumType.GetMember(actualStrategy.ToString());
				var strategyEnumValueMemberInfo = strategyMemberInfos.FirstOrDefault(m => m.DeclaringType == strategyEnumType);

				if (strategyEnumValueMemberInfo.IsDefined(typeof(HasTargetsAttribute), false)) {
					MakePropertyField(targetsScene, "Targets");
					MakePropertyField(applyDampening, "Apply Dampening");
					MakePropertyField(parametersObject.FindProperty("DampeningDistanceThreshold"), "Dampening Distance Threshold");
					MakePropertyField(parametersObject.FindProperty("DampeningRange"), "Dampening Range");
					MakePropertyField(applySmoothing, "Apply Smoothing");
					MakePropertyField(parametersObject.FindProperty("SmoothingFactor"), "Smoothing Factor");
				} else if (actualStrategy == WRStrategy.SteerToOrbit) {
					MakePropertyField(targetsScene, "Targets");

					var steerToOrbitRadius = parametersObject.FindProperty("SteerToOrbitRadius");
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(steerToOrbitRadius, new GUIContent("Steer To Orbit Radius"));
					EditorGUILayout.Space(20);
					EditorGUILayout.LabelField($"Rotation Rate: {(360f / (2 * Mathf.PI * steerToOrbitRadius.floatValue)).ToString("N2")} °/m/s");
					GUILayout.EndHorizontal();

				} else if (actualStrategy == WRStrategy.SteerInDirection) {
					MakePropertyField(direction, "Direction");
				} else if (actualStrategy == WRStrategy.Messinger2019APF) {
					MakePropertyField(parametersObject.FindProperty("SegmentLength"), "Segment Length");
					MakePropertyField(bounds, "Bounds");
				}
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}