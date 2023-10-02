using BG.Redirection;
using UnityEngine;

public class FittsLaw : MonoBehaviour {

	public float a, b;
	private float distance, width;

	private void Start() {
		Scene scene = ((BodyRedirection)Toolkit.Instance.rootScript).scene;
		distance = Vector3.Distance(scene.physicalTarget.position, scene.origin.position);
		width = scene.virtualTarget.localScale.y;

		Debug.Log(computeTime());
	}

	private float computeTime() {
		return a + b * computeIndexOfDifficulty();
	}

	private float computeIndexOfDifficulty() {
		return Mathf.Log(distance / width + 1, 2);
	}
}