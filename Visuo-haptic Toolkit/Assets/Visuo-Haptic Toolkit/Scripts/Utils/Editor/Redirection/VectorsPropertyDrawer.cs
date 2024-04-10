using UnityEngine;
using UnityEditor;

using VHToolkit.Redirection;

public class VectorsPropertyDrawer : PropertyDrawer {
	protected float labelWidth = 60;

	protected float GetWidth(float availableWidth) {
		return availableWidth / 2f - 5f;
	}
}

	[CustomPropertyDrawer(typeof(Vector2Horizontal))]
public class Vector2HorizontalPropertyDrawer : VectorsPropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var rectWidth = GetWidth(EditorGUIUtility.currentViewWidth - position.x);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;

		var leftRect = new Rect(position.x, position.y, rectWidth, position.height);
		var rightRect = new Rect(position.x + rectWidth + 5f, position.y, rectWidth, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("left"), new GUIContent("Left"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("right"), new GUIContent("Right"));

		EditorGUIUtility.labelWidth = width;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Vector2Vertical))]
public class Vector2VerticalPropertyDrawer : VectorsPropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var rectWidth = GetWidth(EditorGUIUtility.currentViewWidth - position.x);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;

		var leftRect = new Rect(position.x, position.y, rectWidth, position.height);
		var rightRect = new Rect(position.x + rectWidth + 5f, position.y, rectWidth, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("up"), new GUIContent("Up"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("down"), new GUIContent("Down"));

		EditorGUIUtility.labelWidth = width;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Vector2Gain))]
public class Vector2GainPropertyDrawer : VectorsPropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var rectWidth = GetWidth(EditorGUIUtility.currentViewWidth - position.x);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;

		var leftRect = new Rect(position.x, position.y, rectWidth, position.height);
		var rightRect = new Rect(position.x + rectWidth + 5f, position.y, rectWidth, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("forward"), new GUIContent("Forward"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("backward"), new GUIContent("Backward"));

		EditorGUIUtility.labelWidth = width;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Vector2Rotation))]
public class Vector2RotationPropertyDrawer : VectorsPropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var rectWidth = GetWidth(EditorGUIUtility.currentViewWidth - position.x);

		var width = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;

		var leftRect = new Rect(position.x, position.y, rectWidth, position.height);
		var rightRect = new Rect(position.x + rectWidth + 5f, position.y, rectWidth, position.height);

		EditorGUI.PropertyField(leftRect, property.FindPropertyRelative("same"), new GUIContent("Same"));
		EditorGUI.PropertyField(rightRect, property.FindPropertyRelative("opposite"), new GUIContent("Opposite"));

		EditorGUIUtility.labelWidth = width;

		EditorGUI.EndProperty();
	}
}
