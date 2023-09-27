using UnityEngine;

/// <summary>
/// This class is used to copy the translations of the trackedObject in the horizontal plane while staying above the object.
/// </summary>
public class TopViewCamera : MonoBehaviour {
	public Transform trackedObject;

	private void Update() {
		this.transform.position = new Vector3(trackedObject.position.x, this.transform.position.y, trackedObject.position.z);
	}
}