using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// This class allows to calibrate a virtual scene with the physical environment using a controller or a tacker (not yet implemented).
/// </summary>
public class SceneCalibration : MonoBehaviour {

	public enum CalibrationState {
		None,
		FirstPoint,
		SecondPoint,
		ThirdPoint,
		Computation
	}

	[SerializeField] private Transform physicalTracker;
	[SerializeField] private Transform[] virtualTrackers;
	[SerializeField] private Vector3 forwardOffset;	// Offset in the (left/right, up/down, forward/backward direction)

	private InputDevice hand;
	[SerializeField] private InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Right;

	private CalibrationState state;
	private Vector3[] points;
	private bool buttonWasPressed;

	private string path = "LoggedData\\";

	private void OnEnable() {
		state = CalibrationState.None;
		points = new Vector3[virtualTrackers.Length];
	}

	private void Update() {
		// Get the controller used to set the 3 calibration points
		List<InputDevice> foundControllers = new List<InputDevice>();
  	    UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(characteristics, foundControllers);
		hand = foundControllers.FirstOrDefault();

		bool buttonPress;
		if (virtualTrackers.Length == 3) {
			switch (state) {
				case CalibrationState.FirstPoint:
					if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
						points[0] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
						Debug.Log("First calibration point saved. Ready for the second point");
						state++;
					}
					break;
				case CalibrationState.SecondPoint:
					if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
						points[1] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
						Debug.Log("Second calibration point saved. Ready for the third point");
						state++;
					}
					break;
				case CalibrationState.ThirdPoint:
					if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
						points[2] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
						Debug.Log("Third calibration point saved.");
						state++;
					}
					break;
				case CalibrationState.Computation:
					virtualTrackers[0].position = points[0];	// Align the first point with the tracker

					// Align the second point by rotating the world
					float angle = Vector3.Angle(virtualTrackers[1].position - virtualTrackers[0].position, points[1] - points[0]);
					Vector3 axis = Vector3.Cross(virtualTrackers[1].position - virtualTrackers[0].position, points[1] - points[0]);
					virtualTrackers[0].Rotate(axis, angle);

					// Align the third point by rotating the world
					angle = Vector3.Angle(virtualTrackers[2].position - virtualTrackers[0].position, points[2] - points[0]);
					axis = Vector3.Cross(virtualTrackers[2].position - virtualTrackers[0].position, points[2] - points[0]);
					virtualTrackers[0].Rotate(axis, angle);

					state = CalibrationState.None;
					SaveCalibration();
					Debug.Log("Calibration done and saved to file.");
					break;
			}
		} else if (virtualTrackers.Length == 1) {
			if ((hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && buttonPress) || Input.GetKeyUp(KeyCode.Space)) {
				virtualTrackers[0].SetPositionAndRotation(physicalTracker.position, physicalTracker.rotation);
            }
		} else {
			Debug.LogWarning($"Incorrect number of virtualTrackers. There should be 1 or 3, there are {virtualTrackers.Length}.");
		}

		buttonWasPressed = hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && buttonPress;
	}

    /// <summary>
    /// This function sets the state of the cvalibration so that the user can start calibration on the next frame
    /// </summary>
    public void Calibrate() => state = CalibrationState.FirstPoint;

    /// <summary>
    /// Logs the transform of the root world to save calibration. The previous calibration can fail if the headset is self tracking and went in sleep mode as this usually resets the tracking origin.
    /// </summary>
    public void SaveCalibration() {
		using (var writer = new StreamWriter($"{path}LastCalibration.txt")) {
			writer.WriteLine(virtualTrackers[0].position);
			writer.WriteLine(virtualTrackers[0].rotation);
		}
		using (var writer = new StreamWriter($"{path}calibration_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt")) {
			writer.WriteLine(virtualTrackers[0].position);
			writer.WriteLine(virtualTrackers[0].rotation);
		}
	}

	/// <summary>
	/// Loads the transform of the root world from the last calibration. The previous calibration can fail if the headset is self tracking and went in sleep mode as this usually resets the tracking origin.
	/// To load another calibration file, rename it to "LastCalibration.txt".
	/// </summary>
	public void LoadCalibration() {
		string[] lines = File.ReadAllLines($"{path}LastCalibration.txt");

		float[] tmpPosition = lines[0][1..^1].Split(", ", 3).Select(x => float.Parse(x)).ToArray();
		float[] tmpRotation = lines[1][1..^1].Split(", ", 4).Select(x => float.Parse(x)).ToArray();
		virtualTrackers[0].SetPositionAndRotation(position: new(tmpPosition[0], tmpPosition[1], tmpPosition[2]), rotation: new(tmpRotation[0], tmpRotation[1], tmpRotation[2], tmpRotation[3]));
    }
}