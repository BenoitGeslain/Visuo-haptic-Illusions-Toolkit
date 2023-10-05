using UnityEngine;

using BG.Redirection;

namespace BG.Trajectory {

	public class BezierTrajectorGenerator : MonoBehaviour {

		private Scene scene;

		private float currentTime = 0f;
		private float movementTime = 2f;

		public bool movementInProgress = false;
		public Vector2 P1;
		private Vector2 P2;

		private Vector3 initialPosition;
		private Vector3 finalPosition;

		private void Start() {
			scene = ((BodyRedirection)Toolkit.Instance.rootScript).scene;

			initialPosition = scene.origin.position;
			finalPosition = scene.physicalTarget.position;
			P2 = Vector2.one;
		}

		private void Update() {
			if (movementInProgress) {
				float d = currentTime / movementTime;
				float t = (P1.x - Mathf.Sqrt(Mathf.Pow(P1.x, 2) + d * (P2.x - 2 * P1.x))) / (2 * P1.x - P2.x);
				if (t < 0 || t > 1) {
					t = (P1.x + Mathf.Sqrt(Mathf.Pow(P1.x, 2) + d * (P2.x - 2 * P1.x))) / (2 * P1.x - P2.x);
				}

				float yPosition = 2 * t * (1 - t) * P1.y + Mathf.Pow(t, 2) * P2.y;
				Debug.Log($"{t} {yPosition} {(finalPosition - initialPosition).x} {(finalPosition - initialPosition).z}");

				scene.physicalHand.position = initialPosition + new Vector3((finalPosition - initialPosition).x * d, 0f, (finalPosition - initialPosition).z * yPosition);

				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}
	}
}