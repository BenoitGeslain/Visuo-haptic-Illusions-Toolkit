using UnityEngine;
using UnityEditor;

namespace VHToolkit.Redirection {
	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(Vector2Horizontal))]
	public class Vector2HorizontalPropertyDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't indent child fields
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var leftRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var rightRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("left"), new GUIContent("Left"));
			EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("right"), new GUIContent("Right"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}


	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(Vector2Vertical))]
	public class Vector2VerticalPropertyDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't indent child fields
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var maxRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("down"), new GUIContent("Down"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("up"), new GUIContent("Up"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}


	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(Vector2Gain))]
	public class Vector2GainPropertyDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't indent child fields
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var maxRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("slower"), new GUIContent("Slower"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("faster"), new GUIContent("Faster"));

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
