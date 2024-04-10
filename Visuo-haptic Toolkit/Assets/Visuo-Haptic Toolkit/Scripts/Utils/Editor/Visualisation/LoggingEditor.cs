using UnityEngine;
using UnityEditor;

namespace VHToolkit.Logging {
	[CustomEditor(typeof(Socket)), CanEditMultipleObjects]
	public class SocketEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Launch visualizer")) {
				(target as Socket).LaunchVisualizer();
			}
		}
	}
}