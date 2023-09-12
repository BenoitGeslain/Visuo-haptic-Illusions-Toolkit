using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	// [CanEditMultipleObjects]
	[CustomEditor(typeof(ParametersToolkit))]
	public class ParametersToolkitEditor: Editor {

		SerializedProperty maxAngles;
		SerializedProperty noRedirectionBuffer;

		SerializedProperty rotationalEpsilon;
		SerializedProperty minimumRotation;
		SerializedProperty smoothingFactor;
		SerializedProperty walkingThreshold;
		SerializedProperty overTimeRotaton;
		SerializedProperty gainsTranslational;
		SerializedProperty gainsRotational;
		SerializedProperty curvatureRadius;

		private bool defaultEditor;

		private void OnEnable() {
			maxAngles = serializedObject.FindProperty("MaxAngles");
			noRedirectionBuffer = serializedObject.FindProperty ("NoRedirectionBuffer");

			rotationalEpsilon = serializedObject.FindProperty("RotationalEpsilon");
			minimumRotation = serializedObject.FindProperty("MinimumRotation");
			smoothingFactor = serializedObject.FindProperty("SmoothingFactor");
			walkingThreshold = serializedObject.FindProperty("WalkingThreshold");
			overTimeRotaton = serializedObject.FindProperty ("OverTimeRotaton");
			gainsTranslational = serializedObject.FindProperty("GainsTranslational");
			gainsRotational = serializedObject.FindProperty ("GainsRotational");
			curvatureRadius = serializedObject.FindProperty("CurvatureRadius");
		}

		public override void OnInspectorGUI() {
			defaultEditor = EditorGUILayout.Toggle("Show Default Editor", defaultEditor);

			if (defaultEditor) {
				base.OnInspectorGUI();
			} else {
				GUI.enabled = false;
				EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject((ParametersToolkit)target), typeof(ParametersToolkit), false);
				GUI.enabled = true;

				serializedObject.Update();

				EditorGUILayout.PropertyField(maxAngles, new GUIContent ("Threshold Angles"));
				EditorGUILayout.PropertyField(noRedirectionBuffer, new GUIContent ("No Redirection Buffer"));

				EditorGUILayout.PropertyField(rotationalEpsilon, new GUIContent ("Rotational Epsilon"));
				EditorGUILayout.PropertyField(minimumRotation, new GUIContent ("Minimum Rotation"));
				EditorGUILayout.PropertyField(smoothingFactor, new GUIContent ("Smoothing Factor"));
				EditorGUILayout.PropertyField(walkingThreshold, new GUIContent ("Walking Threshold"));
				EditorGUILayout.PropertyField(overTimeRotaton, new GUIContent ("Over Time Rotaton"));

				EditorGUILayout.PropertyField(gainsTranslational, new GUIContent ("Translational Gains"));
				EditorGUILayout.PropertyField(gainsRotational, new GUIContent ("Rotational Gains"));

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(curvatureRadius, new GUIContent ("Curvature Radius"));
				EditorGUILayout.LabelField("Rotation Rate: " + (360f / (2 * Mathf.PI * curvatureRadius.floatValue)).ToString("N2") + " °/m/s");
				GUILayout.EndHorizontal();

				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
