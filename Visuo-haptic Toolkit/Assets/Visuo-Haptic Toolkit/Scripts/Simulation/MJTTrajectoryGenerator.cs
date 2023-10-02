using UnityEngine;

using BG.Redirection;

namespace BG.Trajectory {
	public class MJTTrajectoryGenerator : MonoBehaviour {

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

			Debug.Log($"{initialPosition} {finalPosition}");
			Debug.Log($"{MJTEquation(initialPosition.x, finalPosition.x, 0f)} {MJTEquation(initialPosition.x, finalPosition.x, 0.5f)} {MJTEquation(initialPosition.x, finalPosition.x, 1f)}");
			Debug.Log($"{MJTEquation(initialPosition.y, finalPosition.y, 0f)} {MJTEquation(initialPosition.y, finalPosition.y, 0.5f)} {MJTEquation(initialPosition.y, finalPosition.y, 1f)}");
			Debug.Log($"{MJTEquation(initialPosition.z, finalPosition.z, 0f)} {MJTEquation(initialPosition.z, finalPosition.z, 0.5f)} {MJTEquation(initialPosition.z, finalPosition.z, 1f)}");
		}

		private void Update() {
			if (movementInProgress) {
				currentPosition = ComputeMJTPosition(currentTime / movementTime);
				currentPosition.y = scene.physicalHand.position.y;
				scene.physicalHand.position = currentPosition;

				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}

		private Vector3 ComputeMJTPosition(float normailzedTime) {
			return new Vector3(MJTEquation(initialPosition.x, finalPosition.x, normailzedTime),
							   0f,
							   MJTEquation(initialPosition.z, finalPosition.z, normailzedTime));
		}

		private float MJTEquation(float initial, float final, float t) {
			return initial + (final - initial) * (6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3));
		}
	}
}
