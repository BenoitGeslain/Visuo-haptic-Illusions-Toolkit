using UnityEditor;
using UnityEngine;

namespace BG.Redirection {
	[CustomPropertyDrawer(typeof(MinMaxValues))]
	public class MinMaxValuesEditor: PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = 30;
			EditorGUI.indentLevel = 0;

			var minRect = new Rect(position.x, position.y, position.width/2, position.height);
			var maxRect = new Rect(position.x + position.width/2, position.y, position.width/2, position.height);

			EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), new GUIContent("Min: "));
			EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), new GUIContent("Max: "));

			EditorGUI.indentLevel = indent;
			EditorGUIUtility.labelWidth = 0;

			EditorGUI.EndProperty();
		}
	}
}