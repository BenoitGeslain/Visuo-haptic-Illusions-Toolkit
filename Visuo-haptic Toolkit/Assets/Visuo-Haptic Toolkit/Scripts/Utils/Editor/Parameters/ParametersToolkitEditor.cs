using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection {
	// [CanEditMultipleObjects]
	[CustomEditor(typeof(ParametersToolkit))]
	public class ParametersToolkitEditor: Editor {

		SerializedProperty maxAngles;
		SerializedProperty noRedirectionBuffer;
		SerializedProperty maxRedirectionThreshold;
		SerializedProperty resetRedirectionCoeff;
		SerializedProperty goGoActivationDistance;
		SerializedProperty goGoCoefficient;

		SerializedProperty rotationalEpsilon;
		SerializedProperty minimumRotation;
		SerializedProperty walkingThreshold;
		SerializedProperty distanceThreshold;
		SerializedProperty dampeningRange;
		SerializedProperty smoothingFactor;
		SerializedProperty overTimeRotaton;
		SerializedProperty gainsTranslational;
		SerializedProperty gainsRotational;
		SerializedProperty curvatureRadius;

		private void OnEnable() {
			maxAngles = serializedObject.FindProperty("MaxAngles");
			noRedirectionBuffer = serializedObject.FindProperty ("NoRedirectionBuffer");
			maxRedirectionThreshold = serializedObject.FindProperty ("MaxRedirectionThreshold");
			resetRedirectionCoeff = serializedObject.FindProperty ("ResetRedirectionCoeff");
			goGoActivationDistance = serializedObject.FindProperty ("GoGoActivationDistance");
			goGoCoefficient = serializedObject.FindProperty ("GoGoCoefficient");

			rotationalEpsilon = serializedObject.FindProperty("RotationalError");
			minimumRotation = serializedObject.FindProperty("MinimumRotation");
			walkingThreshold = serializedObject.FindProperty("WalkingThreshold");
			distanceThreshold = serializedObject.FindProperty("DistanceThreshold");
			dampeningRange = serializedObject.FindProperty("DampeningRange");
			smoothingFactor = serializedObject.FindProperty("SmoothingFactor");
			overTimeRotaton = serializedObject.FindProperty ("OverTimeRotation");
			gainsTranslational = serializedObject.FindProperty("GainsTranslational");
			gainsRotational = serializedObject.FindProperty ("GainsRotational");
			curvatureRadius = serializedObject.FindProperty("CurvatureRadius");
		}

		// public override void OnInspectorGUI() {
		// 	GUI.enabled = false;
		// 	EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject((ParametersToolkit)target), typeof(ParametersToolkit), false);
		// 	GUI.enabled = true;

		// 	serializedObject.Update();

		// 	// Body Warping
		// 	EditorGUILayout.PropertyField(maxAngles, new GUIContent ("Threshold Angles"));
		// 	EditorGUILayout.PropertyField(noRedirectionBuffer, new GUIContent ("No Redirection Buffer"));
		// 	EditorGUILayout.PropertyField(maxRedirectionThreshold, new GUIContent ("Max Redirection Threshold"));
		// 	EditorGUILayout.PropertyField(resetRedirectionCoeff, new GUIContent ("Reset Redirection Coefficient"));
		// 	EditorGUILayout.PropertyField(goGoActivationDistance, new GUIContent ("GoGo Activation Distance"));
		// 	EditorGUILayout.PropertyField(goGoCoefficient, new GUIContent ("GoGo Coefficient"));

		// 	// World Warping
		// 	EditorGUILayout.PropertyField(rotationalEpsilon, new GUIContent ("Rotational Epsilon"));
		// 	EditorGUILayout.PropertyField(minimumRotation, new GUIContent ("Minimum Rotation"));
		// 	EditorGUILayout.PropertyField(walkingThreshold, new GUIContent ("Walking Threshold"));

		// 	EditorGUILayout.PropertyField(distanceThreshold, new GUIContent ("Distance Threshold"));
		// 	EditorGUILayout.PropertyField(dampeningRange, new GUIContent ("Dampening Range"));
		// 	EditorGUILayout.PropertyField(smoothingFactor, new GUIContent ("Smoothing Factor"));

		// 	EditorGUILayout.PropertyField(overTimeRotaton, new GUIContent ("Over Time Rotaton"));
		// 	EditorGUILayout.PropertyField(gainsTranslational, new GUIContent ("Translational Gains"));
		// 	EditorGUILayout.PropertyField(gainsRotational, new GUIContent ("Rotational Gains"));

		// 	EditorGUILayout.BeginHorizontal();
		// 	EditorGUILayout.PropertyField(curvatureRadius, new GUIContent ("Curvature Radius"));
		// 	EditorGUILayout.LabelField("Rotation Rate: " + (360f / (2 * Mathf.PI * curvatureRadius.floatValue)).ToString("N2") + " °/m/s");
		// 	GUILayout.EndHorizontal();

		// 	serializedObject.ApplyModifiedProperties();
		// }
	}
}
