using UnityEngine;

/// <summary>
/// Stub class for getting Guardian boundaries, if configured.
/// </summary>
public class Guardian : MonoBehaviour {
	private void Update() {
		bool configured = OVRManager.boundary.GetConfigured();
		if (configured) {
			Vector3[] geometry = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
			Debug.Log($"Boundary has {geometry.Length} points.");
		}
	}
}