using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;
using VHToolkit.Redirection;

public class DeviceTracking : MonoBehaviour {

	int counter = 0;

	[SerializeField] private InputDeviceCharacteristics Headset = InputDeviceCharacteristics.HeadMounted;
	Transform physicalHead, virtualHead;
	private Vector3 headsetOffset = Vector3.zero;
	// [SerializeField] private InputDeviceCharacteristics controllerLeft = InputDeviceCharacteristics.Left;
	// private Vector3 offsetControllerLeft;
	// [SerializeField] private InputDeviceCharacteristics controllerRight = InputDeviceCharacteristics.Right;
	// private Vector3 offsetControllerRight;
	// Transform physicalRightHand, virtualRightHand;

	private void OnEnable() {
		physicalHead = GetComponent<Interaction>().scene.physicalHead;
		virtualHead = GetComponent<Interaction>().scene.virtualHead;
		// physicalRightHand = GetComponent<Interaction>().scene.physicalHead;
		// virtualRightHand = GetComponent<Interaction>().scene.physicalHead;
	}

	private void Update() {
		// counter++;
		// Debug.Log(counter);
		// if (counter==120) {
		// 	virtualHead.position = physicalHead.position;
		// }
		// List<InputDevice> foundControllers = new List<InputDevice>();
  	    // UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(Headset, foundControllers);
		// if (foundControllers.FirstOrDefault() != null) {
		// 	headsetOffset = physicalHead.position - virtualHead.position;
		// } else {
		// 	virtualHead.position = physicalHead.position + headsetOffset;
		// }
	}
}