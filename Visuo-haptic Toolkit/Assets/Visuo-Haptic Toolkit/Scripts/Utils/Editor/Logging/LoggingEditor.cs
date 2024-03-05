using UnityEngine;
using UnityEditor;

namespace VHToolkit.Logging {
	[CustomEditor(typeof(Logging)), CanEditMultipleObjects]
	public class CSVLoggingEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Create new log file")) {
				Logging script = (Logging)target;
				script.CreateNewFile();
			}
		}
	}
}