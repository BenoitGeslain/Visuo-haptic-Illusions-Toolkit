using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	[CustomEditor(typeof(BodyRedirection))]
	public class BodyRedirectionEditor : Editor {

		// BodyRedirection bodyRedirection;
		// SerializedProperty redirecting;

		// private void OnEnable() {
		// 	bodyRedirection = serializedObject.targetObject as BodyRedirection;
		// 	redirecting = serializedObject.FindProperty( "redirecting" );

		// }

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			// EditorGUI.PropertyField(new Rect(0, 300, 500, 30), redirecting, new GUIContent("Redirecting"));
		}
	}
}