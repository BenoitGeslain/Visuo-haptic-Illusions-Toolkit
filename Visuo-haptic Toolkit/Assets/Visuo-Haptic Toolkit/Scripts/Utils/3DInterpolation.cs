using UnityEngine;
using System.Collections.Generic;
using VHToolkit;
using System.Linq;

public class Interpolation : MonoBehaviour {
	[SerializeField] private List<Vector3> originalMesh, targetMesh, interpolatedMesh;

	private void Start() {

	}

	private void OnDrawGizmos() {
		var colors = Enumerable.Range(0, originalMesh.Count).Select(t => (float) t).Select( t => Color.Lerp(Color.red, Color.white, t / originalMesh.Count)).ToList();
		foreach (var (v, c) in originalMesh.Zip(colors)) {
			Gizmos.color = c;
			Gizmos.DrawWireSphere(v, 0.2f);
		}
		foreach (var (v, c) in targetMesh.Zip(colors))
		{
			Gizmos.color = c;
			Gizmos.DrawSphere(v, 0.1f);
		}

		var disp2 = ThinPlateSpline.SabooSmoothedDisplacementField(originalMesh.ToArray(), targetMesh.ToArray(), 0f);
		var disp = ThinPlateSpline.SabooDisplacementField(originalMesh.ToArray(), targetMesh.ToArray());
		var tmp2 = interpolatedMesh.ConvertAll(g => disp(g));
		var tmp = interpolatedMesh.ConvertAll(g => disp(g));
		Debug.Log($"tmp {tmp.Zip(tmp2, (a,b) => (a -b).sqrMagnitude).Sum()}");
		foreach (var (v, c) in interpolatedMesh.Zip(colors))
		{
			Gizmos.color = c;
			Gizmos.DrawWireCube(v, new(0.2f,0.2f,0.2f));
		}
		foreach (var (v, c) in tmp.Zip(colors))
		{
			Gizmos.color = c;
			Gizmos.DrawCube(v, new(0.2f, 0.2f, 0.2f));
		}
	}
}