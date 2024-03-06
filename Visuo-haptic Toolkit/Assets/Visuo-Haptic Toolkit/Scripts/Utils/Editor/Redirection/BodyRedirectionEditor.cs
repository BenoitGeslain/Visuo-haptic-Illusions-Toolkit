using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection.BodyRedirection {
	/// <summary>
	/// Custom editor for the body redirection scene. Allows to show the Geslain2022Polynom parameters only if it is selected.
	/// </summary>
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;
		SerializedProperty techniqueInstance;

		SerializedProperty physicalLimbs;
		// SerializedProperty virtualLimbs;
		SerializedProperty physicalHead;
		SerializedProperty virtualHead;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;
		SerializedProperty origin;
		SerializedProperty redirect;

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
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BodyRedirection)target), typeof(BodyRedirection), false);
			GUI.enabled = true;

			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent("Redirection technique"));

			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("User Parameters", EditorStyles.boldLabel);

			// Scene
			EditorGUILayout.PropertyField(physicalLimbs, new GUIContent("Physical Limbs"));

			// EditorGUILayout.PropertyField(virtualLimbs, new GUIContent("Virtual Limbs"));
			if (technique.enumNames[technique.enumValueIndex] == "Azmandian2016Hybrid") {
				EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
				EditorGUILayout.PropertyField(virtualHead, new GUIContent("Virtual Head"));
			}
			else if (technique.enumNames[technique.enumValueIndex] == "Poupyrev1996GoGo") {
				EditorGUILayout.PropertyField(physicalHead, new GUIContent("Physical Head"));
			}


			EditorGUILayout.Space(5);
			EditorGUILayout.LabelField("Technique Parameters", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(redirect, new GUIContent("Redirect"));

			EditorGUILayout.PropertyField(physicalTarget, new GUIContent("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent("Virtual Target"));
			EditorGUILayout.PropertyField(origin, new GUIContent("Origin"));

			// Hides redirectionLateness and controlpoint fields if the technique is not Geslain2022Polynom
			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				EditorGUILayout.PropertyField(techniqueInstance, new GUIContent("Parameters"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}