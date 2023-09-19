using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;


public class HandModelsManager: MonoBehaviour {

	private InputDevice targetDevice;
	private GameObject intantiatedController, instantiatedHand;


	[SerializeField] private bool showController;
	[SerializeField] private InputDeviceCharacteristics deviceCharacteristics;
	[SerializeField] private List<GameObject> modelPrefabs;
	[SerializeField] private Transform trackedHand;

	private void Start() {
		TryInitialize();
	}

	private void Update() {
        if (targetDevice.isValid) {
            intantiatedController.SetActive(showController);
            instantiatedHand.SetActive(!showController);
        }
        else {
            TryInitialize();
        }
    }

	private void TryInitialize() {
		List<InputDevice> devices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices); // Populates devices with all connected devices

		devices.ForEach(device => Debug.Log($"Detected device: {device.name}"));

		if (devices.Count > 0) {
			targetDevice = devices[0];
			GameObject prefab = modelPrefabs.Find(controller => controller.name == targetDevice.name);
			if (prefab) {
				intantiatedController = Instantiate(prefab, trackedHand != null ? trackedHand : transform);
                intantiatedController.transform.localPosition = Vector3.zero;
            }

			instantiatedHand = Instantiate(modelPrefabs[0], trackedHand != null ? trackedHand : transform);
			instantiatedHand.transform.localPosition = Vector3.zero;
		}
	}
}