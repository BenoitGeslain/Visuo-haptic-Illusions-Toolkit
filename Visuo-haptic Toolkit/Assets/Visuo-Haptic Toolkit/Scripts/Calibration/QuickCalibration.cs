using UnityEngine;

public class QuickCalibration : MonoBehaviour {
	[SerializeField] private Transform virtualHead;
	[SerializeField] private Transform physicalHead;
	[SerializeField] private Transform world;

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			world.SetPositionAndRotation(new(physicalHead.position.x, world.position.y, physicalHead.position.z) , Quaternion.Euler(0f, physicalHead.rotation.eulerAngles.y, 0f));
		}
	}
}