using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneCalibration))]
public class SceneCalibrationEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Calibrate")) {
			SceneCalibration script = (SceneCalibration)target;
			script.Calibrate();
		}

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Save Calibration")) {
			SceneCalibration script = (SceneCalibration)target;
			script.SaveCalibration();
		}
		if (GUILayout.Button("Load Last Calibration")) {
			SceneCalibration script = (SceneCalibration)target;
			script.LoadCalibration();
		}
		EditorGUILayout.EndHorizontal();
	}
}