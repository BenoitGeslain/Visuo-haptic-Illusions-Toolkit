using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;
using VHToolkit.Demo;
using VHToolkit.Redirection;

namespace VHToolkit.Calibration {

	public class QuickCalibration : MonoBehaviour {
		[SerializeField] private Transform physicalHead;
		[SerializeField] private Transform world;

		[SerializeField] private InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller & InputDeviceCharacteristics.TrackedDevice;

		private bool wasPressingButton = false;
		private List<InputDevice> inputDevices;
		private WorldRedirection script;

		private void Start() {
			script = Toolkit.Instance.gameObject.GetComponent<WorldRedirection>();
		}

		private void Update() {
			inputDevices = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics(characteristics, inputDevices);

			if (inputDevices.Any()) {
				inputDevices.First().TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonStatus);
				if (buttonStatus && !wasPressingButton) {
					Debug.Log("Button Pressed");
					// script.StartRedirection();
					world.SetPositionAndRotation(new(physicalHead.position.x, world.position.y, physicalHead.position.z) , Quaternion.Euler(0f, physicalHead.rotation.eulerAngles.y, 0f));
					GetComponent<CorridorRedirection>().state++;
				}
				wasPressingButton = buttonStatus;
			}
		}
	}
}