using System.Collections.Generic;

using UnityEngine;

public class CubesRedirection : MonoBehaviour {
	[SerializeField] private Transform realCube;
	[SerializeField] private List<Transform> VirtualCubes;

	private int currentCube;

	private void Start() {

	}

	private void Update() {

	}

	private void NextCube() {
		currentCube = ++currentCube % VirtualCubes.Count;

	}
}