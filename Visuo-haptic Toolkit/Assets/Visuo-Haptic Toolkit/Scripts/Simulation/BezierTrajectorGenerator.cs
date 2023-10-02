using UnityEngine;

using BG.Redirection;

namespace BG.Trajectory {

	public class BezierTrajectorGenerator : MonoBehaviour {

		private Scene scene;

		float currentTime = 0f;
		float movementTime = 2f;

		public bool movementInProgress = false;
		Vector3 initialPosition;
		Vector3 finalPosition;
		Vector3 currentPosition;

		private void Start() {
			scene = ((BodyRedirection)Toolkit.Instance.rootScript).scene;

			initialPosition = scene.origin.position;
			currentPosition = scene.origin.position;
			finalPosition = scene.physicalTarget.position;

		}

		private void Update() {
			if (movementInProgress) {


				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}
	}
}