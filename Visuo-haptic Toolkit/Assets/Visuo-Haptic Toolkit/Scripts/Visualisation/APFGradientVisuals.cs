using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using VHToolkit;

public class GradientVisuals : MonoBehaviour {

	// PROPERTIES //

	// public general
	public string obstacleTab;
	public bool verbose;
	public int refreshRate;

	// public heatmap
	[Header("Heatmap")]
	[InspectorName("Enable Heatmap")] public bool heatmapEnabled;
	public GameObject heatmapQuadPrefab;
	[Range(1, 64)][Tooltip("The higher the finer")] public int heatmapMeshFineness;
	[Range(1, 10)] public float clampValue = 1;


	// public vectors
	[Header("Vector Field")]
	[InspectorName("Enable Vectors")] public bool vectorsEnabled;
	[InspectorName("Arrow Sprite")] public Sprite vfArrow;
	[InspectorName("Sprite Warning")] public Sprite vfWarning;

	// private general
	private Func<Vector3, float> repulsiveFunction;
	private bool heatmapPreviouslyEnabled;
	private bool vectorsPreviouslyEnabled;
	private float stepX, stepZ, width, depth;
	private Vector3 min, max;
	private List<Collider> obstaclesCollider;

	// private heatmap
	private float[] hmDensityTable;
	private Renderer hmRenderer;
	private GameObject hmQuad;

	// private vector
	private GameObject vfPhysicalUser2d;
	private List<GameObject> vfVectors;



	// FUNCS & SUBS //

	// General

	void Start() {
		InvokeRepeating(nameof(CheckOnBools), 3f, 1f);
		hmDensityTable = new float[4096];
		vfVectors = new();

		// vf todo
		vfPhysicalUser2d = GameObject.Find("2duser");
		vfArrow = Resources.Load<Sprite>("gradient/fleche");
		vfWarning = Resources.Load<Sprite>("gradient/warning");
	}

	private void Log(string msg) {
		if (verbose) Debug.Log(msg);
	}

	public void CheckOnBools() {
		if ((!heatmapPreviouslyEnabled && heatmapEnabled) || (!vectorsPreviouslyEnabled && vectorsEnabled)) InitVisualsTransform(!heatmapPreviouslyEnabled && heatmapEnabled, !vectorsPreviouslyEnabled && vectorsEnabled);
		if (heatmapPreviouslyEnabled && !heatmapEnabled) CloseHeatmap();
		if (vectorsPreviouslyEnabled && !vectorsEnabled) CloseVectorField();

		heatmapPreviouslyEnabled = heatmapEnabled;
		vectorsPreviouslyEnabled = vectorsEnabled;
	}

	private void InitVisualsTransform(bool initHeatmap = false, bool initVectors = false) {
		if (!initHeatmap && !initVectors) return;

		obstaclesCollider = new List<Collider>(GameObject.FindGameObjectsWithTag(obstacleTab).Select(o => o.GetComponent<Collider>()));

		if (obstaclesCollider.Any()) {
			static Func<Vector3, Vector3, Vector3> Pointwise(Func<float, float, float> f) =>
				(a, b) => new(f(a.x, b.x), f(a.y, b.y), f(a.z, b.z));

			repulsiveFunction = MathTools.RepulsivePotential3D(obstaclesCollider);

			var allColliderBounds = FindObjectsOfType<Collider>().Select(o => o.bounds.max).ToArray();
			max = allColliderBounds.Aggregate(Pointwise(Mathf.Max));
			min = allColliderBounds.Aggregate(Pointwise(Mathf.Min));
			min.y = max.y = 0f;

			width = max.x - min.x;
			depth = max.z - min.z;

			stepX = width / heatmapMeshFineness;
			stepZ = depth / heatmapMeshFineness;

			Log($"APF GRADIENT VISUALS INIT : X[{min.x}:{max.x}] Z[{min.z}:{max.z}]");
			Log($"APF GRADIENT VISUALS INIT : width:{width} height:{depth}");

			// heatmap
			if (initHeatmap) {
				hmQuad = Instantiate(heatmapQuadPrefab, this.transform);
				hmQuad.transform.position = 0.5f * (min + max);
				hmQuad.transform.localScale = new Vector2(width, depth);
				hmQuad.layer = LayerMask.NameToLayer("Visuals");

				hmRenderer = hmQuad.GetComponent<Renderer>();

				UpdateHeatmap();
				InvokeRepeating(nameof(UpdateHeatmap), 3f, 1f);
			}
			else if (hmQuad != null) {
				hmQuad.transform.position = 0.5f * (min + max) + Vector3.Scale(hmQuad.transform.position, Vector3.up);
				hmQuad.transform.localScale = new Vector2(width, depth);
			}


			// vectors

			int r = 12; // todo
			int i = 0;

			if (initVectors) {
				for (int z = 0; z < r; z++) {
					for (int x = 0; x < r; x++) {
						GameObject vectorObj = new($"vector_{i}");

						Vector3 position = new(min.x + (x * (width / r)) + (width / r / 2), 0, min.z + (z * (depth / r)) + (depth / r));
						Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

						vectorObj.transform.parent = this.transform;
						vectorObj.transform.position = new Vector3(position.x, transform.position.y + 0.001f, position.z);
						vectorObj.AddComponent<SpriteRenderer>();

						if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y)) {
							float angleInDegrees = Mathf.Atan2(gradient.y, gradient.x) * Mathf.Rad2Deg;

							vectorObj.transform.rotation = Quaternion.Euler(-90, 0, angleInDegrees);
							vectorObj.GetComponent<SpriteRenderer>().sprite = vfArrow;
						}
						else {
							vectorObj.GetComponent<SpriteRenderer>().sprite = vfWarning;
						}
						vfVectors.Add(vectorObj);
						i++;
					}
				}

				UpdateVectors();
				InvokeRepeating(nameof(UpdateVectors), 3f, 1f);

			}
		}
		else {
			heatmapEnabled = heatmapPreviouslyEnabled = false;
			Log("No colliders detected, can generate Heatmap.");
		}
	}


	// Heatmap

	public void UpdateHeatmap() {
		if (!heatmapEnabled) { return; }

		stepX = width / heatmapMeshFineness;
		stepZ = depth / heatmapMeshFineness;

		for (int z = 0; z < heatmapMeshFineness; z++) {
			for (int x = 0; x < heatmapMeshFineness; x++) {

				Vector3 position = new Vector3(min.x + (x * stepX) + (stepX / 2), 0, min.z + (z * stepZ) + (stepZ / 2));
				Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

				float hmValue = gradient.magnitude;

				hmDensityTable[x + (z * heatmapMeshFineness)] = hmValue;
				Log($"{stepX * x:0.0} - {stepZ * z:0.0} : {hmValue}");
			}
		}

		float maxDen = clampValue;

		hmRenderer.material.SetFloat("_UnitsPerSide", heatmapMeshFineness);
		hmRenderer.material.SetFloat("_MaxDen", maxDen);
		hmRenderer.material.SetFloatArray("_DensityTable", hmDensityTable);
	}

	public void CloseHeatmap() {
		CancelInvoke(nameof(UpdateHeatmap));
		GameObject.Destroy(hmQuad);
		hmQuad = null;
		hmRenderer = null;
	}


	// Vectors

	public void UpdateVectors() {
		if (!vectorsEnabled) { return; }

		foreach (GameObject obj in vfVectors) {
			Vector2 gradient = MathTools.Gradient3(repulsiveFunction, obj.transform.position);

			var (angleInDegrees, sprite) = (float.IsNaN(gradient.x) || float.IsNaN(gradient.y)) ?
				 (0f, vfWarning) : (Mathf.Atan2(gradient.y, gradient.x) * Mathf.Rad2Deg, vfArrow);
			obj.transform.rotation = Quaternion.Euler(-90f, 0f, angleInDegrees);
			obj.GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}

	private void CloseVectorField() {
		CancelInvoke(nameof(UpdateHeatmap));
		vfVectors.Clear();
	}
}
