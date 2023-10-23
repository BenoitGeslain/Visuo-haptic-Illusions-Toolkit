using UnityEngine;
using System.Collections;
using UnityEditor;

// This class defines the ColorSpacer attribute, so that
// it can be used in your regular MonoBehaviour scripts:

namespace BG.Redirection {
	[CustomPropertyDrawer(typeof(TooltipCustom))]
	public class TooltipPropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent l) {
			GUIContent label = new GUIContent();
			label.image = l.image;
			label.text = l.text;

			label.tooltip = ((TooltipCustom)attribute).text;

			// Save settings
			bool wasEnabled = GUI.enabled;
			int previousIndentLevel = EditorGUI.indentLevel;

			int indentMod = previousIndentLevel - property.depth;
			position.height = 16f;

			SerializedProperty serializedProperty = property.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			EditorGUI.indentLevel = serializedProperty.depth + indentMod;
			bool enterChildren = EditorGUI.PropertyField(position, serializedProperty, label) && serializedProperty.hasVisibleChildren;

			position.y += EditorGUI.GetPropertyHeight(serializedProperty, label, false);
			position.y += 2;

			while (serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty)) {
				EditorGUI.indentLevel = serializedProperty.depth + indentMod;

				EditorGUI.BeginChangeCheck();
				enterChildren = (serializedProperty.hasVisibleChildren && EditorGUI.PropertyField(position, serializedProperty));
				if (EditorGUI.EndChangeCheck()) break;

				position.y += 2;
				position.y += EditorGUI.GetPropertyHeight(serializedProperty);
			}

			// Restore settings
			GUI.enabled = wasEnabled;
			EditorGUI.indentLevel = previousIndentLevel;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUI.GetPropertyHeight(property, label);
		}
	}
}