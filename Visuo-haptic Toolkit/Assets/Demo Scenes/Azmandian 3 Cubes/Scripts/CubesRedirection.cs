using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit.Redirection.BodyRedirection;

public class CubesRedirection : MonoBehaviour {

	public enum CubesState {
		ReachOrigin,
		ReachCube
	}

	[SerializeField] private CubesState state;
	[SerializeField] private int currentCube = 0;

	[SerializeField] private Transform realCube;
	[SerializeField] private List<Transform> VirtualCubes;

	[SerializeField] private Material magic, regular;

	private BodyRedirection script;

	private void Start() {
		script = this.GetComponent<BodyRedirection>();
		VirtualCubes.ForEach(cube => cube.GetComponentInChildren<ParticleSystem>().Stop());
		VirtualCubes[currentCube].GetComponent<MeshRenderer>().material = magic;
	}

	private void Update() {
		switch (state) {
			case CubesState.ReachOrigin:
				if (script.scene.GetPhysicalHandOriginDistance().Any(d => d <= 0.1f)) {
					script.scene.virtualTarget = VirtualCubes[currentCube];
					state = CubesState.ReachCube;
				}
				break;
			case CubesState.ReachCube:
				if (script.scene.GetPhysicalHandTargetDistance().Any(d => d <= 0.1f)) {
					NextCube();
					state = CubesState.ReachOrigin;
				}
				break;
		}
	}

	private void NextCube() {
		VirtualCubes[currentCube].GetComponent<MeshRenderer>().material = regular;
		VirtualCubes[currentCube].GetComponentInChildren<ParticleSystem>().Stop();

		currentCube = (currentCube + 1) % VirtualCubes.Count;

		VirtualCubes[currentCube].GetComponent<MeshRenderer>().material = magic;
		VirtualCubes[currentCube].GetComponentInChildren<ParticleSystem>().Play();

	}
}