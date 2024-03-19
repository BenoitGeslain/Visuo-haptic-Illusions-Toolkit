using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using VHToolkit;

public class Interpolation : MonoBehaviour {
	// [SerializeField] private List<Vector3> originalMesh, targetMesh, interpolatedMesh;

	[SerializeField] private int n;
	[SerializeField] private float l;

	[SerializeField] private Transform original, target, interpolated, modified;

	[SerializeField] private Transform vHand, pHand;

	private Transform[] originalGrid, targetGrid, interpolatedGrid, modifiedGrid;
	private Vector3[] fixedGrid;
	private MeshFilter originalMeshFilter, targetMeshFilter, interpolatedMeshFilter, modifiedMeshFilter;
	private Mesh originalMesh, targetMesh, interpolatedMesh, modifiedMesh;
	private void Start() {
		originalGrid = new Transform[n * n];
		fixedGrid = new Vector3[n * n];
		targetGrid = new Transform[n * n];
		interpolatedGrid = new Transform[n * n];
		modifiedGrid = new Transform[n * n];

		originalMeshFilter = original.GetComponent<MeshFilter>();
		targetMeshFilter = target.GetComponent<MeshFilter>();
		interpolatedMeshFilter = interpolated.GetComponent<MeshFilter>();
		modifiedMeshFilter = modified.GetComponent<MeshFilter>();

		for (var i = 0; i < n; i++) {
			for (var j = 0; j < n; j++) {
				var index = i * n + j;
				originalGrid[index] = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
				originalGrid[index].name = $"Original Point {i} {j}";
				originalGrid[index].parent = original;
				originalGrid[index].position = new(i * l, 0f, j * l);
				originalGrid[index].localScale = new(0.1f, 0.1f, 0.1f);

				fixedGrid[index] = new(i * l, 10f, j * l);

				targetGrid[index] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				targetGrid[index].name = $"target Point {i} {j}";
				targetGrid[index].parent = target;
				targetGrid[index].position = originalGrid[index].position - 0.1f * j * Vector3.down;
				targetGrid[index].localScale = new(0.1f, 0.1f, 0.1f);

				interpolatedGrid[index] = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
				interpolatedGrid[index].name = $"Interpolated Point {i} {j}";
				interpolatedGrid[index].parent = interpolated;
				interpolatedGrid[index].position = new(i * l, 0f, j * l);
				interpolatedGrid[index].localScale = new(0.1f, 0.1f, 0.1f);

				modifiedGrid[index] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				modifiedGrid[index].name = $"Modified Point {i} {j}";
				modifiedGrid[index].parent = modified;
				modifiedGrid[index].position = new(0f, 0f, 0f);
				modifiedGrid[index].localScale = new(0.1f, 0.1f, 0.1f);
			}
		}
	}

	private void Update() {

        // var disp = ThinPlateSpline.SabooSmoothedDisplacementField(
        // 	Array.ConvertAll(originalGrid, g => g.position),
        // 	Array.ConvertAll(targetGrid, g => g.position),
        // 	0,
        // 	false
        // 	);
        Vector3 disp(Vector3 x) => InverseWeightedDistance.Interpolate(
            Array.ConvertAll(originalGrid, g => g.position).Concat(fixedGrid).ToArray(),
            Array.ConvertAll(targetGrid, g => g.position).Concat(fixedGrid).ToArray(),
            2,
            x
        );

        var tmp = Array.ConvertAll(interpolatedGrid, g => disp(g.position));
		DrawMeshes(tmp);

        var colors = Array.ConvertAll(originalMesh.uv, vec => Color.Lerp(Color.red, Color.white, vec.sqrMagnitude));

		originalMesh.colors = colors;
		targetMesh.colors = colors;
		// interpolatedMesh.colors = colors;
		// modifiedMesh.colors = colors;

		vHand.position = InverseWeightedDistance.Interpolate(
			Array.ConvertAll(originalGrid, g => g.position).Concat(fixedGrid).ToArray(),
			Array.ConvertAll(targetGrid, g => g.position).Concat(fixedGrid).ToArray(),
			2,
			pHand.position
		);
	}

	private void DrawMeshes(Vector3[] m) {
		Array.ForEach(originalGrid, p => {
			originalMesh = originalMeshFilter.mesh;

			int[] triangles = new int[(n - 1) * (n - 1) * 6];
			for (int ti = 0, x = 0, y = 0; ti + 6 <= triangles.Length; x++, ti += 6) {
				if (x == n - 1) {
					y++; x = 0;
				}
				triangles[ti] = x + y * n;
				triangles[ti + 4] = triangles[ti + 1] = x + y * n + 1;
				triangles[ti + 3] = triangles[ti + 2] = x + y * n + n;
				triangles[ti + 5] = x + y * n + n + 1;
			}
			originalMesh.vertices = Array.ConvertAll(originalGrid, p => p.position);
			originalMesh.triangles = triangles;
			originalMesh.uv = Array.ConvertAll(originalGrid, p => new Vector2(p.position.x, p.position.z) / ((n - 1) * l));
			originalMesh.normals = Array.ConvertAll(originalGrid, _ => Vector3.up);
		});

		targetMesh = targetMeshFilter.mesh;
		targetMesh.vertices = Array.ConvertAll(targetGrid, p => p.position);
		targetMesh.triangles = originalMesh.triangles;
		targetMesh.uv = Array.ConvertAll(targetGrid, p => new Vector2(p.position.x, p.position.z) / ((n - 1) * l));
		targetMesh.normals = Array.ConvertAll(targetGrid, _ => Vector3.up);

		// interpolatedMesh = interpolatedMeshFilter.mesh;
		// interpolatedMesh.vertices = Array.ConvertAll(interpolatedGrid, p => p.position);
		// interpolatedMesh.triangles = originalMesh.triangles;
		// interpolatedMesh.uv = Array.ConvertAll(interpolatedGrid, p => new Vector2(p.position.x, p.position.z) / ((n - 1) * l));
		// interpolatedMesh.normals = Array.ConvertAll(interpolatedGrid, _ => Vector3.up);

		// modifiedMesh = modifiedMeshFilter.mesh;
		// modifiedMesh.vertices = m;
		// modifiedMesh.triangles = modifiedMesh.triangles;
		// modifiedMesh.uv = Array.ConvertAll(m, p => new Vector2(p.x, p.z) / ((n - 1) * l));
		// modifiedMesh.normals = Array.ConvertAll(m, _ => Vector3.up);
		// for (int i = 0; i < m.Length; i++) {
		// 	modifiedGrid[i].position = m[i];
		// }
	}

	// private void OnDrawGizmos() {
	// 	var colors = Enumerable.Range(0, originalMesh.Count).Select(t => (float) t).Select( t => Color.Lerp(Color.red, Color.white, t / originalMesh.Count)).ToList();
	// 	foreach (var (v, c) in originalMesh.Zip(colors)) {
	// 		Gizmos.color = c;
	// 		Gizmos.DrawWireSphere(v, 0.2f);
	// 	}
	// 	foreach (var (v, c) in targetMesh.Zip(colors))
	// 	{
	// 		Gizmos.color = c;
	// 		Gizmos.DrawSphere(v, 0.1f);
	// 	}

	// 	var disp2 = ThinPlateSpline.SabooSmoothedDisplacementField(originalMesh.ToArray(), targetMesh.ToArray(), 0f);
	// 	var disp = ThinPlateSpline.SabooDisplacementField(originalMesh.ToArray(), targetMesh.ToArray());
	// 	var tmp2 = interpolatedMesh.ConvertAll(g => disp(g));
	// 	var tmp = interpolatedMesh.ConvertAll(g => disp(g));
	// 	Debug.Log($"tmp {tmp.Zip(tmp2, (a,b) => (a -b).sqrMagnitude).Sum()}");
	// 	foreach (var (v, c) in interpolatedMesh.Zip(colors))
	// 	{
	// 		Gizmos.color = c;
	// 		Gizmos.DrawWireCube(v, new(0.2f,0.2f,0.2f));
	// 	}
	// 	foreach (var (v, c) in tmp.Zip(colors))
	// 	{
	// 		Gizmos.color = c;
	// 		Gizmos.DrawCube(v, new(0.2f, 0.2f, 0.2f));
	// 	}
	// }

}