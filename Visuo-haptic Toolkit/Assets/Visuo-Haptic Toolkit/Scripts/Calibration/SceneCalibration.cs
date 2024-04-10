using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

namespace VHToolkit.Calibration {
	/// <summary>
	/// This class allows to calibrate a virtual scene with the physical environment using a controller or a tacker (not yet implemented).
	/// </summary>
	public class SceneCalibration : MonoBehaviour {

		enum CalibrationState {
			None,
			FirstPoint,
			SecondPoint,
			ThirdPoint,
			Computation
		}

		[SerializeField] private Transform physicalTracker;
		[SerializeField] private Transform[] virtualTrackers;

		/// Offset in the (left/right, up/down, forward/backward direction)
		[SerializeField] private Vector3 forwardOffset;

		private InputDevice hand;
		[SerializeField] private InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Right;

		private CalibrationState state;
		private Vector3[] points;
		private bool buttonWasPressed;

		private readonly string loggingPath = "LoggedData\\";

		private void Start() {
			state = CalibrationState.None;
			points = new Vector3[virtualTrackers.Length];
		}

		private void Update() {
			// Get the controller used to set the 3 calibration points
			List<InputDevice> foundControllers = new();
			InputDevices.GetDevicesWithCharacteristics(characteristics, foundControllers);
			hand = foundControllers.FirstOrDefault();
			Debug.Log($"found hand: {hand.characteristics}");

			bool buttonPress;
			if (virtualTrackers.Length == 3) {
				switch (state) {
					case CalibrationState.FirstPoint:
						Debug.Log(hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress));
						if (hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
							points[0] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
							Debug.Log("First calibration point saved. Ready for the second point");
							state++;
						}
						break;
					case CalibrationState.SecondPoint:
						if (hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
							points[1] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
							Debug.Log("Second calibration point saved. Ready for the third point");
							state++;
						}
						break;
					case CalibrationState.ThirdPoint:
						if (hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonWasPressed) {
							points[2] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
							Debug.Log("Third calibration point saved.");
							state++;
						}
						break;
					case CalibrationState.Computation:
						virtualTrackers[0].position = points[0];    // Align the first point with the tracker
																	// Align the second and third points by rotating the world
						for (int i = 1; i <= 2; i++) {
							var v = virtualTrackers[i].position - virtualTrackers[0].position;
							var p = points[i] - points[0];
							virtualTrackers[0].Rotate(axis: Vector3.Cross(v, p), angle: Vector3.Angle(v, p));
						}

						state = CalibrationState.None;
						SaveCalibration();
						Debug.Log("Calibration done and saved to file.");
						break;
				}
			}
			else if (virtualTrackers.Length == 1) {
				if (state == CalibrationState.FirstPoint && ((hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress) && buttonPress) || Input.GetKeyUp(KeyCode.Space))) {
					virtualTrackers[0].SetPositionAndRotation(physicalTracker.position, physicalTracker.rotation);
					state++;
				}
			}
			else {
				Debug.LogWarning($"Incorrect number of virtualTrackers. There should be 1 or 3, there are {virtualTrackers.Length}.");
			}

			buttonWasPressed = hand.TryGetFeatureValue(CommonUsages.triggerButton, out buttonPress) && buttonPress;
		}

		/// <summary>
		/// This function sets the state of the calibration so that the user can start calibration on the next frame
		/// </summary>
		public void Calibrate() => state = CalibrationState.FirstPoint;

		/// <summary>
		/// Logs the transform of the root world to save calibration. The previous calibration can fail if the headset is self tracking and went in sleep mode as this usually resets the tracking origin.
		/// </summary>
		public void SaveCalibration() {
			string[] filenames = { $"{loggingPath}LastCalibration.txt", $"{loggingPath}calibration_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt" };
			virtualTrackers[0].GetPositionAndRotation(out var position, out var rotation);
			foreach (var filename in filenames) {
				using var writer = new StreamWriter(filename);
				writer.WriteLine(position);
				writer.WriteLine(rotation);
			}
		}

		/// <summary>
		/// Loads the transform of the root world from the last calibration. The previous calibration can fail if the headset is self tracking and went in sleep mode as this usually resets the tracking origin.
		/// To load another calibration file, rename it to "LastCalibration.txt".
		/// </summary>
		public void LoadCalibration() {
			string[] lines = File.ReadAllLines($"{loggingPath}LastCalibration.txt");

			float[] tmpPosition = Array.ConvertAll(lines[0][1..^1].Split(", ", 3), float.Parse);
			float[] tmpRotation = Array.ConvertAll(lines[1][1..^1].Split(", ", 4), float.Parse);
			virtualTrackers[0].SetPositionAndRotation(position: new(tmpPosition[0], tmpPosition[1], tmpPosition[2]), rotation: new(tmpRotation[0], tmpRotation[1], tmpRotation[2], tmpRotation[3]));
		}
	}
}