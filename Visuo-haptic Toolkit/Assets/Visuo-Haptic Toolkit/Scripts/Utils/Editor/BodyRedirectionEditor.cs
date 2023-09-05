using System;

using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {
		SerializedProperty technique;

		SerializedProperty physicalHand;
		SerializedProperty virtualHand;
		SerializedProperty origin;
		SerializedProperty physicalTarget;
		SerializedProperty virtualTarget;

		float a0, a1, a2;

		private void OnEnable() {
			technique = serializedObject.FindProperty ("technique");

			physicalHand = serializedObject.FindProperty ("physicalHand");
			virtualHand = serializedObject.FindProperty ("virtualHand");
			origin = serializedObject.FindProperty ("origin");
			physicalTarget = serializedObject.FindProperty ("physicalTarget");
			virtualTarget = serializedObject.FindProperty ("virtualTarget");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.PropertyField(technique, new GUIContent ("Technique"));

			if (technique.enumNames[technique.enumValueIndex] == "Geslain2022Polynom") {
				BodyRedirection bodyRedirection = (BodyRedirection)target;
				try {
					Geslain2022Polynom techniqueInstance = (Geslain2022Polynom)bodyRedirection.techniqueInstance;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel("Polynom Coefficients");
					EditorGUIUtility.labelWidth = 20;
					a0 = EditorGUILayout.FloatField("a0", a0, GUILayout.Width(100));
					a1 = EditorGUILayout.FloatField("a1", a1, GUILayout.Width(100));
					a2 = EditorGUILayout.FloatField("a2", a2, GUILayout.Width(100));
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.EndHorizontal();

					if (techniqueInstance != null) {
						techniqueInstance.a0 = a0;
						techniqueInstance.a1 = a1;
						techniqueInstance.a2 = a2;
					}
				}
				catch (InvalidCastException e) {}

			}

			EditorGUILayout.PropertyField(physicalHand, new GUIContent ("Physical Hand"));
			EditorGUILayout.PropertyField(virtualHand, new GUIContent ("Virtual Hand"));
			EditorGUILayout.PropertyField(origin, new GUIContent ("Origin"));
			EditorGUILayout.PropertyField(physicalTarget, new GUIContent ("Physical Target"));
			EditorGUILayout.PropertyField(virtualTarget, new GUIContent ("Virtual Target"));

			serializedObject.ApplyModifiedProperties();
			// base.OnInspectorGUI();

			// EditorGUILayout.BeginHorizontal();
			// if(GUILayout.Button("Reset Redirection")) {
			// 	BodyRedirection BRScript = (BodyRedirection) target;
			// 	BRScript.resetRedirection();
			// }
			// // if(GUILayout.Button("Restart Redirection")) {
			// // 	BodyRedirection BRScript = (BodyRedirection) target;
			// // 	BRScript.restartRedirection();
			// // }
			// EditorGUILayout.EndHorizontal();
		}
	}
}