using UnityEngine;
using UnityEditor;

namespace VHToolkit.Calibration {
	/// <summary>
	/// Custom <c>Editor</c> for <c>Scene</c> calibration.
	/// </summary>
	[CustomEditor(typeof(SceneCalibration))]
	public class SceneCalibrationEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Calibrate")) {
				(target as SceneCalibration).Calibrate();
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save Calibration")) {
				(target as SceneCalibration).SaveCalibration();
			}
			if (GUILayout.Button("Load Last Calibration")) {
				(target as SceneCalibration).LoadCalibration();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}