using UnityEngine;
using UnityEditor;

namespace BG.Redirection {
	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(XDirection))]
	public class XDirectionPropertyDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 60;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width / 2, position.height);
			var maxRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("left"), new GUIContent("Left"));
			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("right"), new GUIContent("Right"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(YDirection))]
	public class YDirectionPropertyDrawer : PropertyDrawer
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

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("up"), new GUIContent("Up"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("down"), new GUIContent("Down"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(ZDirection))]
	public class ZDirectionPropertyDrawer : PropertyDrawer
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

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("forward"), new GUIContent("Forward"));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("backward"), new GUIContent("Backward"));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}
}
