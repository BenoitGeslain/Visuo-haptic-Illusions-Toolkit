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
	[SerializeField] private int activeCubeIndex = 0;

	[SerializeField] private Transform realCube;
	[SerializeField] private List<Transform> VirtualCubes;

	[SerializeField] private Material magic, regular;

	[SerializeField] private BodyRedirection script;

	private void Start() {
		// script = this.GetComponent<BodyRedirection>();
		VirtualCubes.ForEach(cube => cube.GetComponentInChildren<ParticleSystem>().Stop());
		VirtualCubes[activeCubeIndex].GetComponent<MeshRenderer>().material = magic;
		VirtualCubes[activeCubeIndex].GetComponent<TargetCollider>().enableTrigger = true;
	}

	public void TouchedOrigin() {
		if (state == CubesState.ReachOrigin) {
			script.scene.virtualTarget = VirtualCubes[activeCubeIndex];
			state = CubesState.ReachCube;
		}
	}

	public void TouchedTarget() {
		if (state == CubesState.ReachCube) {
			NextCube();
			state = CubesState.ReachOrigin;
		}
	}

	private void NextCube() {
		VirtualCubes[activeCubeIndex].GetComponent<MeshRenderer>().material = regular;
		VirtualCubes[activeCubeIndex].GetComponentInChildren<ParticleSystem>().Stop();
		VirtualCubes[activeCubeIndex].GetComponent<TargetCollider>().enableTrigger = false;

		activeCubeIndex = (activeCubeIndex + 1) % VirtualCubes.Count;

		VirtualCubes[activeCubeIndex].GetComponent<MeshRenderer>().material = magic;
		VirtualCubes[activeCubeIndex].GetComponentInChildren<ParticleSystem>().Play();
		VirtualCubes[activeCubeIndex].GetComponent<TargetCollider>().enableTrigger = true;
	}
}