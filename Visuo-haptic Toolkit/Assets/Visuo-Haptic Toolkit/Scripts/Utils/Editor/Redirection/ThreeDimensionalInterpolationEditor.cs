using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection.Interpolation3D {
	/// <summary>
	/// Custom editor for the 3DInterpolation Monobehaviour.
	/// </summary>
	[CustomEditor(typeof(ThreeDimensionalInterpolation))]
	public class ThreeDimensionalInterpolationEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalLimbs;
		SerializedProperty reference;
		SerializedProperty interpolated;

		SerializedProperty redirect;
		SerializedProperty parameters;
		SerializedObject parametersObject;

		private void Start() {

			technique = serializedObject.FindProperty("_technique");
			techniqueInstance = serializedObject.FindProperty("techniqueInstance");

			physicalLimbs = serializedObject.FindProperty("scene.limbs");
			reference = serializedObject.FindProperty("scene.reference");
			interpolated = serializedObject.FindProperty("scene.interpolated");
			redirect = serializedObject.FindProperty("redirect");

			parameters = serializedObject.FindProperty("scene.parameters");
		}

		private void MakePropertyField(SerializedProperty property, string text, string tooltip = null) {
			EditorGUILayout.PropertyField(property, new GUIContent(text, tooltip));
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ThreeDimensionalInterpolation)target), typeof(ThreeDimensionalInterpolation), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.largeLabel);

			Debug.Log(reference);
			// Scene
			MakePropertyField(physicalLimbs, "Physical Limbs", "The Transform of the user's limbs tracked by the VR SDK");


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.largeLabel);

			string techniqueName = technique.enumNames[technique.enumValueIndex];
			Debug.Log(serializedObject);

			if (techniqueName == nameof(Kohli2010RedirectedTouching)) {
				MakePropertyField(reference, "Reference Points", "The reference array for the unmodified object");
				MakePropertyField(interpolated, "Interpolated Points", "The interpolated array for the modified object");
			}

			MakePropertyField(technique, "Redirection technique", "");

			MakePropertyField(redirect, "Activate Redirection", "");
			MakePropertyField(parameters, "Numerical Parameters", "");

			// If no parameters Scriptable object, update object and don't render the rest of the view
			if (parameters.objectReferenceValue == null)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}

			parametersObject = new SerializedObject(parameters.objectReferenceValue);
			parametersObject.Update();

			if (techniqueName == nameof(Kohli2010RedirectedTouching)) {
				MakePropertyField(parametersObject.FindProperty("smoothingParameter"), "Smoothing Parameter");
				MakePropertyField(parametersObject.FindProperty("rescale"), "Rescale");
			}

			serializedObject.ApplyModifiedProperties();
			parametersObject.ApplyModifiedProperties();
		}
	}
}