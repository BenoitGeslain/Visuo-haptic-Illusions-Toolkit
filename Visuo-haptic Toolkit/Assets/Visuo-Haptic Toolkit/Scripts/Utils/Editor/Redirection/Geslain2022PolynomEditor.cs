using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection {
	[CustomPropertyDrawer(typeof(BodyRedirectionTechnique))]
	public class BodyRedirectionTechniquePropertyDrawer: PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			// Property redirectionLateness
			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.PropertyField(position, property.FindPropertyRelative("redirectionLateness"));

			EditorGUI.EndProperty();

			// New line
			Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);

			// Property controlPoint
			EditorGUI.BeginProperty(rect, label, property);

			EditorGUI.PropertyField(rect, property.FindPropertyRelative("controlPoint"));

			EditorGUI.EndProperty();
		}
	}
}