// using UnityEngine;
// using UnityEditor;

// namespace BG.Redirection {
// 	[CanEditMultipleObjects]
// 	[CustomEditor(typeof(ParametersToolkit))]
// 	public class ParametersToolkitEditor: Editor {

// 		SerializedProperty maxAngles;
// 		SerializedProperty noRedirectionBuffer;

// 		SerializedProperty rotationalEpsilon;
// 		SerializedProperty overTimeRotaton;
// 		SerializedProperty gainsTranslational;
// 		SerializedProperty gainsRotational;
// 		SerializedProperty gainsCurvature;


// 		private void OnEnable() {
// 			maxAngles = serializedObject.FindProperty("MaxAngles");
// 			noRedirectionBuffer = serializedObject.FindProperty ("NoRedirectionBuffer");

// 			rotationalEpsilon = serializedObject.FindProperty("RotationalEpsilon");
// 			overTimeRotaton = serializedObject.FindProperty ("OverTimeRotaton");
// 			gainsTranslational = serializedObject.FindProperty("GainsTranslational");
// 			gainsRotational = serializedObject.FindProperty ("GainsRotational");
// 			gainsCurvature = serializedObject.FindProperty("GainsCurvature");
// 		}

// 		public override void OnInspectorGUI() {
// 			// base.OnInspectorGUI();

// 			serializedObject.Update();

// 			EditorGUILayout.PropertyField(maxAngles, new GUIContent ("Maximum Angles"));
// 			EditorGUILayout.PropertyField(noRedirectionBuffer, new GUIContent ("No Redirection Buffer"));

// 			EditorGUILayout.PropertyField(rotationalEpsilon, new GUIContent ("Rotational Epsilon"));
// 			EditorGUILayout.PropertyField(overTimeRotaton, new GUIContent ("Over Time Rotaton"));

// 			EditorGUILayout.BeginHorizontal();
// 			EditorGUILayout.PrefixLabel("Gains Translational");
// 			Debug.Log(gainsTranslational.vector2Value);
// 			EditorGUIUtility.labelWidth = 50;
// 			EditorGUILayout.FloatField("Forward", gainsTranslational.vector2Value.x);
// 			EditorGUIUtility.labelWidth = 60;
// 			EditorGUILayout.FloatField("Backward", gainsTranslational.vector2Value.y);
// 			EditorGUILayout.EndHorizontal();
// 			EditorGUILayout.PropertyField(gainsRotational, new GUIContent ("Gains Rotational"));
// 			EditorGUILayout.PropertyField(gainsCurvature, new GUIContent ("Gains Curvature"));

// 			serializedObject.ApplyModifiedProperties();
// 		}
// 	}
// }
