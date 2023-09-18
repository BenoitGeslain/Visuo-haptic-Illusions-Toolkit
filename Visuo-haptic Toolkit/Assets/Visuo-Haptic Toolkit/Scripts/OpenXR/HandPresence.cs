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
		if (!targetDevice.isValid) {
			TryInitialize();
		} else {
			if (showController) {
				intantiatedController.SetActive(true);
				instantiatedHand.SetActive(false);
			} else {
				intantiatedController.SetActive(false);
				instantiatedHand.SetActive(true);
			}
		}
	}

	private void TryInitialize() {
		List<InputDevice> devices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, devices);	// Populates devices with all connected devices

		foreach (var d in devices) {
			Debug.Log("Detected device: " + d.name);
		}

		if (devices.Count>0) {
			targetDevice = devices[0];
			GameObject prefab = modelPrefabs.Find(controller => controller.name == targetDevice.name);
			if (prefab) {
				if (trackedHand != null) {
					intantiatedController = Instantiate(prefab, trackedHand);
					intantiatedController.transform.localPosition = Vector3.zero;

				} else {
					intantiatedController = Instantiate(prefab, transform);
					intantiatedController.transform.localPosition = Vector3.zero;
				}
			}

			if (trackedHand != null) {
				instantiatedHand = Instantiate(modelPrefabs[0], trackedHand);
				instantiatedHand.transform.localPosition = Vector3.zero;
			} else {
				instantiatedHand = Instantiate(modelPrefabs[0], transform);
				instantiatedHand.transform.localPosition = Vector3.zero;
			}
		}
	}
}