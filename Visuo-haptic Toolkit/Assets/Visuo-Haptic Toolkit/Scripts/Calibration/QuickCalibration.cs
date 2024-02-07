using UnityEngine;

namespace VHToolkit.Calibration {

	public class QuickCalibration : MonoBehaviour {
		[SerializeField] private Transform physicalHead;
		[SerializeField] private Transform world;

        private void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("Resetting world position");
				world.SetPositionAndRotation(new(physicalHead.position.x, 0f, physicalHead.position.z),
											 Quaternion.Euler(0f, physicalHead.rotation.eulerAngles.y, 0f));
			}
		}
	}
}