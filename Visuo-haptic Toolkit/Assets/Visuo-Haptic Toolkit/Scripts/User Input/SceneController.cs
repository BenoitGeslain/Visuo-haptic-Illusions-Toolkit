using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[System.Serializable]

public class SceneController : MonoBehaviour {
	[SerializeField] private GameObject toolkitRoot;
	[SerializeField] private GameObject resetCubesButton;

	[SerializeField] private List<Transform> cubes;
	[SerializeField] private Material pressedMaterial;
	private Material unpressedMaterial;
	private MeshRenderer mesh;

	private List<Vector3> cubesInitialPosition;

	private void Start() {
		mesh = GetComponent<MeshRenderer>();
		unpressedMaterial = mesh.material;
		cubesInitialPosition = cubes.Select(cube => cube.position).ToList();
	}

	private void OnTriggerEnter(Collider other) {
		mesh.material = pressedMaterial;
		ResetCubes();
		Debug.Log("Collider Enter");
	}

	private void OnTriggerExit(Collider other) {
		mesh.material = unpressedMaterial;
		Debug.Log("Collider Exit");
	}

	public void ResetCubes() {
		Debug.Log("reset");
		Enumerable.Zip(cubes, cubesInitialPosition, (c, cP) => (c, cP)).ForEach(z => z.c.position = z.cP);
	}
}