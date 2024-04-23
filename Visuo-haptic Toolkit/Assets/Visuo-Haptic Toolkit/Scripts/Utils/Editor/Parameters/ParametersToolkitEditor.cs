using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection {
	// [CanEditMultipleObjects]
	/// <summary>
	/// Custom <c>Editor</c> for toolkit parameters.
	/// </summary>
	[CustomEditor(typeof(ParametersToolkit))]
	public class ParametersToolkitEditor: Editor {

		// Body Warping
		SerializedProperty horizontalAngles;
		SerializedProperty verticalAngles;
		SerializedProperty depthGain;
		SerializedProperty redirectionBuffer;
		SerializedProperty redirectionLateness;
		SerializedProperty controlPoint;
		SerializedProperty goGoActivationDistance;
		SerializedProperty goGoCoefficient;
		SerializedProperty resetRedirectionCoeff;

		// World Warping
		SerializedProperty rotationalError;
		SerializedProperty rotationalThreshold;
		SerializedProperty walkingThreshold;
		SerializedProperty dampeningDistanceThreshold;
		SerializedProperty dampeningRange;
		SerializedProperty smoothingFactor;
		SerializedProperty steerToOrbitRadius;
		SerializedProperty overTimeRotaton;
		SerializedProperty gainsTranslational;
		SerializedProperty gainsRotational;
		SerializedProperty curvatureRadius;
		SerializedProperty hybridWeights;
		SerializedProperty smoothingParameter;
		SerializedProperty rescale;
		SerializedProperty swampSquareLength;
		SerializedProperty swampCDRatio;

		private void OnEnable() {
			// Body Warping
			horizontalAngles = serializedObject.FindProperty("HorizontalAngles");
			verticalAngles = serializedObject.FindProperty("VerticalAngles");
			depthGain = serializedObject.FindProperty("DepthGain");
			redirectionBuffer = serializedObject.FindProperty("RedirectionBuffer");
			redirectionLateness = serializedObject.FindProperty("RedirectionLateness");
			controlPoint = serializedObject.FindProperty ("ControlPoint");

			goGoCoefficient = serializedObject.FindProperty ("GoGoCoefficient");
			goGoActivationDistance = serializedObject.FindProperty("GoGoActivationDistance");
			resetRedirectionCoeff = serializedObject.FindProperty("ResetRedirectionCoeff");

			// World Warping
			rotationalError = serializedObject.FindProperty("RotationalError");
			rotationalThreshold = serializedObject.FindProperty("RotationalThreshold");
			walkingThreshold = serializedObject.FindProperty("WalkingThreshold");
			dampeningDistanceThreshold = serializedObject.FindProperty("DampeningDistanceThreshold");
			dampeningRange = serializedObject.FindProperty("DampeningRange");

			smoothingFactor = serializedObject.FindProperty("SmoothingFactor");

			steerToOrbitRadius = serializedObject.FindProperty("SteerToOrbitRadius");

			overTimeRotaton = serializedObject.FindProperty ("OverTimeRotation");
			gainsTranslational = serializedObject.FindProperty("GainsTranslational");
			gainsRotational = serializedObject.FindProperty ("GainsRotational");
			curvatureRadius = serializedObject.FindProperty("CurvatureRadius");
			hybridWeights = serializedObject.FindProperty("HybridWeights");

			// 3DInterpolation
			smoothingParameter = serializedObject.FindProperty("SmoothingParameter");
			rescale = serializedObject.FindProperty("Rescale");

			// Pseudo-haptic
			swampSquareLength = serializedObject.FindProperty("SwampSquareLength");
			swampCDRatio = serializedObject.FindProperty("SwampCDRatio");
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject((ParametersToolkit)target), typeof(ParametersToolkit), false);
			GUI.enabled = true;

			serializedObject.Update();

			// Body Warping
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Body/Hand Redirection", EditorStyles.largeLabel);

			EditorGUILayout.PropertyField(horizontalAngles, new GUIContent ("Max Horizontal Angles"));
			EditorGUILayout.PropertyField(verticalAngles, new GUIContent ("Max Vertical Angles"));
			EditorGUILayout.PropertyField(depthGain, new GUIContent("Max Depth Gain"));
			EditorGUILayout.PropertyField(redirectionBuffer, new GUIContent ("No Redirection Buffer"));
			EditorGUILayout.PropertyField(redirectionLateness, new GUIContent ("Redirection Lateness"));
			EditorGUILayout.PropertyField(controlPoint, new GUIContent("Control Point"));
			EditorGUILayout.PropertyField(resetRedirectionCoeff, new GUIContent ("Reset Redirection Coefficient"));
			EditorGUILayout.PropertyField(goGoActivationDistance, new GUIContent ("GoGo Activation Distance"));
			EditorGUILayout.PropertyField(goGoCoefficient, new GUIContent ("GoGo Coefficient"));

			// World Warping
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("World Redirection / Redirected Walking", EditorStyles.largeLabel);

			EditorGUILayout.PropertyField(rotationalError, new GUIContent ("Rotational Error"));
			EditorGUILayout.PropertyField(rotationalThreshold, new GUIContent ("Rotational Threshold"));
			EditorGUILayout.PropertyField(walkingThreshold, new GUIContent ("Walking Threshold"));

			EditorGUILayout.PropertyField(dampeningDistanceThreshold, new GUIContent ("Dampening Distance Threshold"));
			EditorGUILayout.PropertyField(dampeningRange, new GUIContent("Dampening Range"));
			EditorGUILayout.PropertyField(smoothingFactor, new GUIContent ("Smoothing Factor"));

			EditorGUILayout.PropertyField(steerToOrbitRadius, new GUIContent("SteerToOrbit Radius"));

			EditorGUILayout.PropertyField(overTimeRotaton, new GUIContent ("Over Time Rotaton"));
			EditorGUILayout.PropertyField(gainsTranslational, new GUIContent ("Translational Gains"));
			EditorGUILayout.PropertyField(gainsRotational, new GUIContent ("Rotational Gains"));

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(curvatureRadius, new GUIContent ("Curvature Radius"));
			EditorGUILayout.Space(20);
			EditorGUILayout.LabelField("   Rotation Rate: " + (360f / (2 * Mathf.PI * curvatureRadius.floatValue)).ToString("N2") + " °/m/s");
			GUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(hybridWeights, new GUIContent("Hybrid Sum Weights"));

			// 3DInterpolation
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("3D Interpolation", EditorStyles.largeLabel);
			EditorGUILayout.PropertyField(smoothingParameter, new GUIContent("Smoothing Parameter"));
			EditorGUILayout.PropertyField(rescale, new GUIContent("Rescale"));

			// Pseudo-haptic
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Pseudo-haptic", EditorStyles.largeLabel);
			EditorGUILayout.PropertyField(swampSquareLength, new GUIContent("Swamp Square Length"));
			EditorGUILayout.PropertyField(swampCDRatio, new GUIContent("Swamp CD Ratio"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
