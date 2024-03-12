using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using VHToolkit.Redirection.PseudoHaptics;

namespace VHToolkit.Redirection.BodyRedirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalLimbs;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty redirect;
		SerializedProperty parameters;
		SerializedObject parametersObject;

		readonly HashSet<string> bufferTechniques = new() { nameof(Han2018InterpolatedReach), nameof(Azmandian2016Body), nameof(Geslain2022Polynom), nameof(Cheng2017Sparse) };
		readonly HashSet<string> noThresholdTechniques = new() { nameof(Poupyrev1996GoGo), nameof(Lecuyer2000Swamp), nameof(Samad2019Weight) };


		private void OnEnable() {

			technique = serializedObject.FindProperty("_technique");
			techniqueInstance = serializedObject.FindProperty("techniqueInstance");

			physicalLimbs = serializedObject.FindProperty("scene.limbs");
			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			redirect = serializedObject.FindProperty("redirect");

			parameters = serializedObject.FindProperty("scene.parameters");
			parametersObject = new SerializedObject(parameters.objectReferenceValue);
		}

		private void MakePropertyField(SerializedProperty property, string text, string tooltip = null) {
			EditorGUILayout.PropertyField(property, new GUIContent(text, tooltip));
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BodyRedirection)target), typeof(BodyRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.largeLabel);

			// Scene
			MakePropertyField(physicalLimbs, "Physical Limbs", "The Transform of the user's limbs tracked by the VR SDK");

			string techniqueName = technique.enumNames[technique.enumValueIndex];

			if (techniqueName == nameof(Azmandian2016Hybrid)) {
				MakePropertyField(physicalHead, "Physical Head", "The Transform of the VR headset worn by the user.");
				MakePropertyField(virtualHead, "Virtual Head", "");
			} else if (techniqueName == nameof(Poupyrev1996GoGo)) {
				MakePropertyField(physicalHead, "Physical Head", "");
			}


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.largeLabel);

			MakePropertyField(technique, "Redirection technique", "");

			MakePropertyField(redirect, "Activate Redirection", "");
			MakePropertyField(parameters, "Numerical Parameters", "");

			// If no parameters Scriptable object, update object and don't render the rest of the view
			if (parameters.objectReferenceValue == null) {
				serializedObject.ApplyModifiedProperties();
				return;
			}

			parametersObject = new SerializedObject(parameters.objectReferenceValue);
			parametersObject.Update();

			MakePropertyField(physicalTarget, "Physical Target", "");
			MakePropertyField(virtualTarget, "Virtual Target", "");
			MakePropertyField(origin, "Origin", "");

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (techniqueName == nameof(Geslain2022Polynom)) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("redirectionLateness"), new GUIContent("Redirection Lateness (a2)"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("controlPoint"), new GUIContent("ControlPoint"));
			} else if (techniqueName == nameof(Poupyrev1996GoGo)) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GoGoCoefficient"), new GUIContent("Coefficient"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("GoGoActivationDistance"), new GUIContent("Activation Distance"));
			}

			if (bufferTechniques.Contains(techniqueName)) {
				EditorGUILayout.PropertyField(parametersObject.FindProperty("RedirectionBuffer"), new GUIContent("Redirection Buffer"));
			}

			if (noThresholdTechniques.Contains(techniqueName)) {
				if (techniqueName == nameof(Lecuyer2000Swamp)) {
					EditorGUILayout.PropertyField(parametersObject.FindProperty("SwampSquareLength"), new GUIContent("Square Side Length"));
					EditorGUILayout.PropertyField(parametersObject.FindProperty("SwampCDRatio"), new GUIContent("C/D Ratio"));
				}
			} else {
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Threshold Parameters", EditorStyles.largeLabel);
				EditorGUILayout.PropertyField(parametersObject.FindProperty("HorizontalAngles"), new GUIContent("Max Horizontal Angles"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty("VerticalAngles"), new GUIContent("Max Vertical Angles"));
				EditorGUILayout.PropertyField(parametersObject.FindProperty(nameof(ParametersToolkit.DepthGain)), new GUIContent("Max Depth Gain"));
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}