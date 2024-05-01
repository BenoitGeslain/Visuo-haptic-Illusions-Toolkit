using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VHToolkit;
using VHToolkit.Redirection.WorldRedirection;

public class GradientVisuals : MonoBehaviour {

	// PROPERTIES //
	[Range(0.5f, 5f)] public float refreshRate;

	// public heatmap
	[Header("Heatmap")]
	[InspectorName("Enable Heatmap")] public bool heatmapEnabled;
	public GameObject heatmapQuadPrefab;
	[Range(1, 64)][Tooltip("The higher the finer")] public int heatmapMeshFineness;
	[Range(1, 10)] public float heatmapClampValue = 1;

	// public vectors
	[Header("Vector Field")]
	[InspectorName("Enable Vectors")] public bool vectorsEnabled;
	[Range(2, 15)] public int vectorMeshFineness;
	[InspectorName("Arrow Sprite")] public Sprite vfArrow;
	[InspectorName("Warning Sprite")] public Sprite vfWarning;

	// private general
	private Func<Vector3, float> repulsiveFunction;
	private float width, depth;
	private Vector3 min, max;
	private List<Collider> obstaclesCollider;

	// private heatmap
	private bool hmEnabledCurrentState;
	private float[] hmDensityTable;
	private Renderer hmRenderer;
	private GameObject hmQuad;

	// private vector
	private bool vfEnabledCurrentState;
	private List<GameObject> vfVectors;

	[SerializeField] private WorldRedirection script;


	// FUNCS & SUBS //

	#region "Main"

	void Start() {
		hmDensityTable = new float[4096];
		vfVectors = new();
		UpdateRepulsiveFunc();
		InvokeRepeating(nameof(Checks), 3f, 3f);
	}


	/// <summary>
	/// Checks a number of things : scene updates, bool changes. If changes are to be made, does them.
	/// </summary>
	private void Checks() {
		bool hmToggled = heatmapEnabled != hmEnabledCurrentState;
		bool vfToggled = vectorsEnabled != vfEnabledCurrentState;

		// check for changes in the scene
		bool updateVisuals = CheckForSceneUpdate();
		if (updateVisuals) UpdateRepulsiveFunc();

		// if nothing to change, return
		if (!hmToggled && !vfToggled && !updateVisuals) return;

		if (hmToggled) {
			if (heatmapEnabled) { heatmapEnabled = InitHeatmap(); }
			else { CloseHeatmap(); }
		}
		else if (updateVisuals && hmEnabledCurrentState) {
			heatmapEnabled = InitHeatmap();
		}

		if (vfToggled) {
			if (vectorsEnabled) { vectorsEnabled = InitVectorField(); }
			else { CloseVectorField(); }
		}
		else if (updateVisuals && vfEnabledCurrentState) {
			vectorsEnabled = InitVectorField();
		}

		// save state
		hmEnabledCurrentState = heatmapEnabled;
		vfEnabledCurrentState = vectorsEnabled;
	}

	/// <summary>
	/// Updates the repulsive function used to calculate gradient.
	/// </summary>
	private void UpdateRepulsiveFunc() {
		// cancel invokes while repulsive function update is ongoing
		CancelInvoke(nameof(UpdateHeatmap));
		CancelInvoke(nameof(UpdateVectorField));

		// get gameobjects with obstacle tag
		obstaclesCollider = ((APFP2R)script.strategyInstance).colliders;

		// Compute the bounding box and repulsive function for all colliders
		if (obstaclesCollider.Any()) {
			repulsiveFunction = MathTools.RepulsivePotential3D(obstaclesCollider);

			var colliders = FindObjectsOfType<Collider>();
			var bounds = new Bounds(colliders.First().transform.position, Vector3.zero);
			foreach (var collider in colliders) {
				bounds.Encapsulate(collider.bounds);
			}
			max = bounds.max;
			min = bounds.min;
			min.y = max.y = 0f;

			width = max.x - min.x;
			depth = max.z - min.z;
		}
		else {
			repulsiveFunction = (x) => 0;
		}
	}

	// TODO : change to SceneUpdate and call it manually when scene is changed
	/// <summary>
	/// Checks for changes with gameobjects tagged as obstacles, and also for changes is scene area size.
	/// </summary>
	/// <returns>True if any change found</returns>
	private bool CheckForSceneUpdate() => true;

	#endregion

	#region "heatmap"

	//TODO : always returns true, should be void?
	/// <summary>
	/// Init heatmap quad with shahder, and invokes update func.
	/// </summary>
	/// <returns>True if init ended sucessfully</returns>
	private bool InitHeatmap() {

		if (hmEnabledCurrentState) {
			CloseHeatmap();
		}

		var hmStepX = width / heatmapMeshFineness;
		var hmStepZ = depth / heatmapMeshFineness;

		hmQuad = Instantiate(heatmapQuadPrefab, this.transform);
		hmQuad.transform.position = 0.5f * (min + max);
		hmQuad.transform.localScale = new Vector2(width, depth);
		hmQuad.layer = LayerMask.NameToLayer("Visuals");

		hmRenderer = hmQuad.GetComponent<Renderer>();

		UpdateHeatmap();
		InvokeRepeating(nameof(UpdateHeatmap), 1f, refreshRate);

		return true;
	}

	/// <summary>
	/// Heatmap quad update method.
	/// </summary>
	private void UpdateHeatmap() {
		if (!heatmapEnabled) { return; }

		var hmStepX = width / heatmapMeshFineness;
		var hmStepZ = depth / heatmapMeshFineness;

		for (int z = 0; z < heatmapMeshFineness; z++) {
			for (int x = 0; x < heatmapMeshFineness; x++) {
				Vector3 position = new(min.x + (x + 1 / 2) * hmStepX, 0, min.z + (z + 1 / 2) * hmStepZ);
				hmDensityTable[x + (z * heatmapMeshFineness)] = ((Vector2)MathTools.Gradient3(repulsiveFunction, position)).magnitude;
			}
		}

		hmRenderer.material.SetFloat("_UnitsPerSide", heatmapMeshFineness);
		hmRenderer.material.SetFloat("_MaxDen", heatmapClampValue);
		hmRenderer.material.SetFloatArray("_DensityTable", hmDensityTable);
	}

	/// <summary>
	/// Handles ending the heatmap.
	/// </summary>
	private void CloseHeatmap() {
		CancelInvoke(nameof(UpdateHeatmap));
		GameObject.Destroy(hmQuad);
		hmQuad = null;
		hmRenderer = null;
	}

	#endregion

	#region "Vector field"

	/// <summary>
	/// Init vector field, and invokes update function.
	/// </summary>
	/// <returns>True if init ended sucessfully</returns>
	private bool InitVectorField() {
		if (repulsiveFunction == null) {
			Debug.Log("APF visuals : cannot init vectors field, no repulsive function (is there any obstacle with collider ?).");
			CloseVectorField();
			return false;
		}

		if (vfEnabledCurrentState) {
			CloseVectorField();
		}

		int i = 0;
		for (int z = 0; z < vectorMeshFineness; z++) {
			for (int x = 0; x < vectorMeshFineness; x++) {
				GameObject vectorObj = new($"vector_{i++}", typeof(SpriteRenderer));

				Vector3 position = new(min.x + (x + 1 / 2) * (width / vectorMeshFineness), 0, min.z + ((z + 1 / 2) * (depth / vectorMeshFineness)));
				Vector2 gradient = MathTools.Gradient3(repulsiveFunction, position);

				vectorObj.transform.parent = this.transform;
				vectorObj.transform.position = new(position.x, transform.position.y + 0.001f, position.z);

				if (!float.IsNaN(gradient.x) && !float.IsNaN(gradient.y)) {
					float angleInDegrees = Mathf.Atan2(gradient.y, gradient.x) * Mathf.Rad2Deg;
					vectorObj.transform.rotation = Quaternion.Euler(-90, 0, angleInDegrees);
					vectorObj.GetComponent<SpriteRenderer>().sprite = vfArrow;
				}
				else {
					vectorObj.GetComponent<SpriteRenderer>().sprite = vfWarning;
				}

				vfVectors.Add(vectorObj);
			}
		}

		UpdateVectorField();
		InvokeRepeating(nameof(UpdateVectorField), 1f, refreshRate);

		return true;
	}

	/// <summary>
	/// Vectors field update method.
	/// </summary>
	private void UpdateVectorField() {
		if (!vectorsEnabled) { return; }

		foreach (GameObject obj in vfVectors) {
			Vector2 gradient = MathTools.Gradient3(repulsiveFunction, obj.transform.position);

			var (angleInDegrees, sprite) = (float.IsNaN(gradient.x) || float.IsNaN(gradient.y)) ?
				 (0f, vfWarning) : (Mathf.Atan2(gradient.y, gradient.x) * Mathf.Rad2Deg, vfArrow);

			obj.transform.rotation = Quaternion.Euler(-90f, 0f, angleInDegrees);
			obj.GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}

	/// <summary>
	/// Handles the ending of the vector field.
	/// </summary>
	private void CloseVectorField() {
		CancelInvoke(nameof(UpdateVectorField));
		vfVectors.Clear();
	}

	#endregion
}
