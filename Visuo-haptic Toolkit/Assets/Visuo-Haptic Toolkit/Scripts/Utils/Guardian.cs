using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit;

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

	static private List<(Vector3, Vector3)> BarrierSegments() {
		var geometry = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
		return geometry.Zip(geometry.Skip(1).Append(geometry.First())).ToList();
	}
}

