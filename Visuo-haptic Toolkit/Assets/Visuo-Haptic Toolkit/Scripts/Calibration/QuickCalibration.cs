using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using VHToolkit.Redirection;

public class QuickCalibration : MonoBehaviour {
	[SerializeField] private Transform physicalHead;
	[SerializeField] private Transform world;

	[SerializeField] private InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller & InputDeviceCharacteristics.TrackedDevice;

	private bool wasPressingButton = false;
	private List<InputDevice> inputDevices;
	private WorldRedirection script;

	private void Start() {
		script = Toolkit.Instance.transform.GetComponent<WorldRedirection>();
	}

	private void Update() {
		List<InputDevice> i = new();
		InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.TrackedDevice, i);
		foreach (var item in i) {
			Debug.Log(item.characteristics);
		}


		inputDevices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(characteristics, inputDevices);

		if (inputDevices.Count > 0) {
			var device = inputDevices[0];

            device.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonStatus);
            if (buttonStatus && !wasPressingButton) {
				script.StartRedirection();
				world.SetPositionAndRotation(new(physicalHead.position.x, world.position.y, physicalHead.position.z) , Quaternion.Euler(0f, physicalHead.rotation.eulerAngles.y, 0f));
			}
			wasPressingButton = buttonStatus;
		}
	}
}