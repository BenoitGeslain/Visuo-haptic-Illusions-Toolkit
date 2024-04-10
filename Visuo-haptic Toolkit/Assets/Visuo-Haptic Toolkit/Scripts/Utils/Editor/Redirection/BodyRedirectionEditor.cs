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

		SerializedProperty limbs;
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

			limbs = serializedObject.FindProperty("scene.limbs");
			physicalHead = serializedObject.FindProperty("scene.physicalHead");
			virtualHead = serializedObject.FindProperty("scene.virtualHead");
			physicalTarget = serializedObject.FindProperty("scene.physicalTarget");
			virtualTarget = serializedObject.FindProperty("scene.virtualTarget");
			origin = serializedObject.FindProperty("scene.origin");
			redirect = serializedObject.FindProperty("redirect");

			parameters = serializedObject.FindProperty("scene.parameters");
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
			MakePropertyField(limbs, "User Limbs", "A list of tracked user limbs.");

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
				MakePropertyField(parametersObject.FindProperty("redirectionLateness"), "Redirection Lateness (a2)");
				MakePropertyField(parametersObject.FindProperty("controlPoint"), "ControlPoint");
			} else if (techniqueName == nameof(Poupyrev1996GoGo)) {
				MakePropertyField(parametersObject.FindProperty("GoGoCoefficient"), "Coefficient");
				MakePropertyField(parametersObject.FindProperty("GoGoActivationDistance"), "Activation Distance");
			}

			if (bufferTechniques.Contains(techniqueName)) {
				MakePropertyField(parametersObject.FindProperty("RedirectionBuffer"), "Redirection Buffer");
			}

			if (noThresholdTechniques.Contains(techniqueName)) {
				if (techniqueName == nameof(Lecuyer2000Swamp)) {
					MakePropertyField(parametersObject.FindProperty("SwampSquareLength"), "Square Side Length");
					MakePropertyField(parametersObject.FindProperty("SwampCDRatio"), "C/D Ratio");
				}
			} else {
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Threshold Parameters", EditorStyles.largeLabel);
				MakePropertyField(parametersObject.FindProperty(nameof(ParametersToolkit.HorizontalAngles)), "Max Horizontal Angles");
				MakePropertyField(parametersObject.FindProperty(nameof(ParametersToolkit.VerticalAngles)), "Max Vertical Angles");
				MakePropertyField(parametersObject.FindProperty(nameof(ParametersToolkit.DepthGain)), "Max Depth Gain");
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}