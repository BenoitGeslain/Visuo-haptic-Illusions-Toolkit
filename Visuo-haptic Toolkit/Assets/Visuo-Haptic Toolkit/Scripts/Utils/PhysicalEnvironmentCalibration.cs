using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit;

public class PhysicalEnvironmentCalibration : MonoBehaviour {

	[SerializeField] private float eps = 0.01f;
	[SerializeField] private Transform user;
	[SerializeField] private List<Vector3> bounds;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		var obs = bounds.CyclicPairs().SelectMany(
			p => {
				var dir = (p.Item1 - p.Item2).normalized;
				var n = (int)(Vector3.Distance(p.Item1, p.Item2) / eps);

				return Enumerable.Range(0, n).Select(j => p.Item2 + (j + 0.5f) * eps * dir);
			}
		).ToList();

		// Debug.Log(obs.Count);
		// obs.ForEach(o => Debug.Log(o));

		Debug.DrawRay(user.position, ComputeGradient(user, obs).normalized);
	}

	private void OnDrawGizmos() {
		foreach (var (first, second) in bounds.CyclicPairs()) {
			Gizmos.DrawLine(first, second);
		}
	}

	private Vector3 ComputeGradient(Transform user, List<Vector3> dividedObstacles) => Vector3.ProjectOnPlane(MathTools.Gradient3(
										MathTools.RepulsivePotential3D(dividedObstacles),
										MathTools.ProjectToHorizontalPlaneV3(user.position)
									), Vector3.up);
}
