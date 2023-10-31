using UnityEngine;
using UnityEditor;

namespace VHToolkit.Logging {
	[CustomEditor(typeof(Logging)), CanEditMultipleObjects]
	public class LoggingEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Create new logging file")) {
				Logging script = (Logging)target;
				script.createNewFile();
			}
		}
	}
}