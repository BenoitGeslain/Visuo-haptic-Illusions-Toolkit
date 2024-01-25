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
                ((SceneCalibration)target).Calibrate();
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save Calibration")) {
                ((SceneCalibration)target).SaveCalibration();
			}
			if (GUILayout.Button("Load Last Calibration")) {
                ((SceneCalibration)target).LoadCalibration();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}