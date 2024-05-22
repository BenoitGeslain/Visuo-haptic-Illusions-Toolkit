using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using VHToolkit.Redirection.PseudoHaptics;
using VHToolkit.Redirection.Interpolation3D;
using System;
using VHToolkit.Redirection;
using System.Linq;

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
		SerializedProperty referenceSurface;
		SerializedProperty interpolatedSurface;
		SerializedProperty redirect;
		SerializedProperty parameters;
		SerializedObject parametersObject;
		private SerializedProperty Find(string name) => serializedObject.FindProperty(name);

		private void OnEnable() {

			technique = Find("_technique");
			techniqueInstance = Find("techniqueInstance");

			limbs = Find("scene.limbs");
			physicalHead = Find("scene.physicalHead");
			virtualHead = Find("scene.virtualHead");
			physicalTarget = Find("scene.physicalTarget");
			virtualTarget = Find("scene.virtualTarget");
			origin = Find("scene.origin");
			referenceSurface = Find("scene.referenceParent");
			interpolatedSurface = Find("scene.interpolatedParent");

			redirect = Find("redirect");
			parameters = Find("scene.parameters");
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

			Enum.TryParse(technique.enumNames[technique.enumValueIndex], out BRTechnique actualTechnique);

			var enumType = typeof(BRTechnique);
			var memberInfos = enumType.GetMember(actualTechnique.ToString());
			var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
			if (enumValueMemberInfo.IsDefined(typeof(ShowHeadAttribute), false)) {
				MakePropertyField(physicalHead, "Physical Head", "The Transform of the VR headset worn by the user.");
			}
			if (actualTechnique == BRTechnique.Azmandian2016Hybrid) {
				MakePropertyField(virtualHead, "Virtual Head", "");
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

			if (actualTechnique == BRTechnique.Kohli2010RedirectedTouching) {
				MakePropertyField(referenceSurface, "Reference Surface", "Points on the physical surface that the physical hand of the user explores. This Transform should be the parent of the points. The order of points is matched with the order of interpolated surface.");
				MakePropertyField(interpolatedSurface, "Interpolated Surface", "Points on the virtual surface that the virtual hand of the user will explore when touching the physical surface.This Transform should be the parent of the points. The order of points is matched with the order of reference surface.");
			}
			else {
				MakePropertyField(physicalTarget, "Physical Target", "");
				MakePropertyField(virtualTarget, "Virtual Target", "");
				MakePropertyField(origin, "Origin", "");
			}


			if (actualTechnique == BRTechnique.Kohli2010RedirectedTouching) {
				MakePropertyField(parametersObject.FindProperty("SmoothingParameter"), "Smoothing", "");
				MakePropertyField(parametersObject.FindProperty("Rescale"), "Rescale", "");
			}
			else if (actualTechnique == BRTechnique.Geslain2022Polynom) {
				// TODO broken
				//MakePropertyField(parametersObject.FindProperty("redirectionLateness"), "Redirection Lateness (a2)");
				//MakePropertyField(parametersObject.FindProperty("controlPoint"), "ControlPoint");
			}
			else if (actualTechnique == BRTechnique.Poupyrev1996GoGo) {
				MakePropertyField(parametersObject.FindProperty("GoGoCoefficient"), "Coefficient");
				MakePropertyField(parametersObject.FindProperty("GoGoActivationDistance"), "Activation Distance");
			}

			if (enumValueMemberInfo.IsDefined(typeof(HasBufferAttribute), false)) {
				MakePropertyField(parametersObject.FindProperty("RedirectionBuffer"), "Redirection Buffer");
			}
			if (actualTechnique == BRTechnique.Lecuyer2000Swamp) {
				MakePropertyField(parametersObject.FindProperty("SwampSquareLength"), "Square Side Length");
				MakePropertyField(parametersObject.FindProperty("SwampCDRatio"), "C/D Ratio");
			}
			if (enumValueMemberInfo.IsDefined(typeof(HasThresholdAttribute), false)) {
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