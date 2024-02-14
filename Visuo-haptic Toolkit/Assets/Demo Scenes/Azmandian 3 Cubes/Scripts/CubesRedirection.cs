using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;

public class CubesRedirection : MonoBehaviour {

	public enum CubesState {
		ReachOrigin,
		ReachCube

	}

	[SerializeField] private CubesState state;
	[SerializeField] private int currentCube;

	[SerializeField] private Transform realCube;
	[SerializeField] private List<Transform> VirtualCubes;

	[SerializeField] private Material magic, regular;

	private BodyRedirection script;

	private void Start() {
		script = this.GetComponent<BodyRedirection>();
	}

	private void Update() {
		switch (state) {
			case CubesState.ReachOrigin:
				if (script.scene.GetPhysicalHandOriginDistance().Any(d => d <= 0.02f)) {
					currentCube = (currentCube + 1) % VirtualCubes.Count;
					script.scene.virtualTarget = VirtualCubes[currentCube];
					state = CubesState.ReachCube;
				}
				break;
			case CubesState.ReachCube:
				if (script.scene.GetPhysicalHandTargetDistance().Any(d => d <= 0.02f)) {
					// currentCube = (currentCube + 1) % VirtualCubes.Count;
					state = CubesState.ReachOrigin;
				}
				break;
		}
	}

	private void NextCube() {
		VirtualCubes[currentCube].GetComponent<MeshRenderer>().material = regular;
		var emission = VirtualCubes[currentCube].GetComponentInChildren<ParticleSystem>().emission;
		emission.enabled = false;

		currentCube = ++currentCube % VirtualCubes.Count;

		VirtualCubes[currentCube].GetComponent<MeshRenderer>().material = magic;
		emission = VirtualCubes[currentCube].GetComponentInChildren<ParticleSystem>().emission;
		emission.enabled = true;

	}
}