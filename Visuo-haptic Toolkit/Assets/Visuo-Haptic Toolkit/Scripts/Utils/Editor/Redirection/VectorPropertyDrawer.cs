using UnityEngine;
using UnityEditor;

using VHToolkit.Redirection;

[CustomPropertyDrawer(typeof(Vector2Horizontal))]
public class Vector2HorizontalPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 50;
		var leftRect = new Rect(position.x, position.y, 125, position.height);
		var rightRect = new Rect(position.x + 130, position.y, 100, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("left"), new GUIContent("Left"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("right"), new GUIContent("Right"));

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Vector2Vertical))]
public class Vector2VerticalPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 50;
		var leftRect = new Rect(position.x, position.y, 125, position.height);
		var rightRect = new Rect(position.x + 130, position.y, 100, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("up"), new GUIContent("Up"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("down"), new GUIContent("Down"));

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Vector2Gain))]
public class Vector2GainPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 50;
		var leftRect = new Rect(position.x, position.y, 125, position.height);
		var rightRect = new Rect(position.x + 130, position.y, 100, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("forward"), new GUIContent("Forward"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("backward"), new GUIContent("Backward"));

		EditorGUI.EndProperty();
	}
}
