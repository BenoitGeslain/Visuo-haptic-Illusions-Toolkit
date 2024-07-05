using UnityEngine;
using UnityEditor;

namespace VHToolkit.Logging
{
    [CustomEditor(typeof(JsonLogging)), CanEditMultipleObjects]
    public class JsonLoggingEditor : Editor
    {
        SerializedProperty logDirectoryPath, optionalFilenamePrefix,
            filename, pythonPath;
        private void OnEnable()
        {
            logDirectoryPath = serializedObject.FindProperty("logDirectoryPath");
            optionalFilenamePrefix = serializedObject.FindProperty("optionalFilenamePrefix");
            filename = serializedObject.FindProperty("filename");
            pythonPath = serializedObject.FindProperty("pythonPath");
        }
        private void MakePropertyField(SerializedProperty property, string text, string tooltip = null)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(text, tooltip));
        }

        public override void OnInspectorGUI()
        { 
            GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((JsonLogging) target), typeof(JsonLogging), false);
			GUI.enabled = true;

			serializedObject.Update();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Json logging Parameters", EditorStyles.largeLabel);

            MakePropertyField(logDirectoryPath, "Log directory Path", "The directory path inside which json file(s) will be created");
            MakePropertyField(optionalFilenamePrefix, "Optional filename prefix", "Optional prefix for the json file");

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("TCP Socket Parameters", EditorStyles.largeLabel);

            MakePropertyField(filename, "Python visualizer script", "File name for the Python visualization script.");
            MakePropertyField(pythonPath, "Python Path", "File name for the Python executable path.");
        }
    }
}