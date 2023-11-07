using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

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
	private Vector3[] points;
	[SerializeField] private Vector3 forwardOffset;	// Offset in the (left/right, up/down, forward/backward direction)

	[SerializeField] private CalibrationState state;

	private string path = "LoggedData\\";
	InputDevice hand;
	bool buttonPressed;

	private void OnEnable() {
		state = CalibrationState.None;
		points = new Vector3[3];
	}

	private void Update() {
		List<InputDevice> foundControllers = new List<InputDevice>();
  	    UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, foundControllers);
		hand = foundControllers.FirstOrDefault();
		bool buttonPress;
		switch (state) {
			case CalibrationState.FirstPoint:
				if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonPressed) {
					points[0] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
					state++;
					// StartCoroutine(incrementState(CalibrationState.SecondPoint));
				}
				break;
			case CalibrationState.SecondPoint:
				if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonPressed) {
					points[1] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
					state++;
					// StartCoroutine(incrementState(CalibrationState.ThirdPoint));
				}
				break;
			case CalibrationState.ThirdPoint:
				if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && !buttonPress && buttonPressed) {
					points[2] = physicalTracker.position + forwardOffset.z * physicalTracker.forward;
					state++;
					// StartCoroutine(incrementState(CalibrationState.Computation));
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
				break;
		}
		buttonPressed = hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out buttonPress) && buttonPress;
	}

	public void Calibrate() {
		state = CalibrationState.FirstPoint;
	}

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

	public void LoadCalibration() {
		string[] lines = File.ReadAllLines($"{path}LastCalibration.txt");

		float[] tmpPosition = lines[0][1..^1].Split(", ", 3).Select(x => float.Parse(x)).ToArray();
		virtualTrackers[0].position = new(tmpPosition[0], tmpPosition[1], tmpPosition[2]);

		float[] tmpRotation = lines[1][1..^1].Split(", ", 4).Select(x => float.Parse(x)).ToArray();
		virtualTrackers[0].rotation = new(tmpRotation[0], tmpRotation[1], tmpRotation[2], tmpRotation[3]);
	}

    private IEnumerator incrementState(CalibrationState targetState) {
		yield return new WaitForSeconds(2f);
		state = targetState;
	}
}