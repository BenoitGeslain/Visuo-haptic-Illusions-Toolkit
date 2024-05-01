using UnityEngine;

/// <summary>
/// This <c>MonoBehaviour</c> is used to copy the movements of the <c>trackedObject</c> Transform
/// in the horizontal plane (<c>x</c> and <c>z</c> coordinates) while leaving the vertical (<c>y</c>)
/// coordinate unchanged.
/// </summary>
public class TopViewCamera : MonoBehaviour {
	public Transform trackedObject;

	private void Update() => transform.position.Set(trackedObject.position.x, transform.position.y, trackedObject.position.z);
}