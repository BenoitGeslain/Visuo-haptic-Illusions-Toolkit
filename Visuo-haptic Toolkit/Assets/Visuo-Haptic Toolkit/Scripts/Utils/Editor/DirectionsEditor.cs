using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(Vector2Translation))]
	public class Vector2TranslationPropertyDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't indent child fields
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var maxRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("forward"), new GUIContent("Forward"));
			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("backward"), new GUIContent("Backward"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(Vector2Rotation))]
	public class Vector2RotationPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var maxRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("same"), new GUIContent("Same"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("opposite"), new GUIContent("Opposite"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}
}
