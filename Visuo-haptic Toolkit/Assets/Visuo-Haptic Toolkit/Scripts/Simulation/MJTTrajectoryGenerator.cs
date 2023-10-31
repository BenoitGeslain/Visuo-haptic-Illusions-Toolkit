using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit.Trajectory {
	public class MJTTrajectoryGenerator : MonoBehaviour {

		private Scene scene;

		float currentTime = 0f;
		float movementTime = 2f;

		public bool movementInProgress = false;
		[Range(0f, 1f)]
		public float ballisticAdjustementPhase = 0.5f;
		Vector3 initialPosition;
		Vector3 finalPositionPhysical;
		Vector3 finalPositionVirtual;
		Vector3 currentPosition;

		private void Start() {
			scene = GetComponent<BodyRedirection>().scene;

			initialPosition = scene.origin.position;
			currentPosition = scene.origin.position;
			finalPositionPhysical = scene.physicalTarget.position;
			finalPositionVirtual = scene.virtualTarget.position;

		}

		private void Update() {
			if (movementInProgress) {
				// currentPosition = ComputeMJTPosition(currentTime / movementTime);
				currentPosition = 0.25f * quadraticTrajectory(currentTime / movementTime) + ComputeMJTPosition(currentTime / movementTime);
				currentPosition.y = scene.physicalHand.position.y;
				scene.physicalHand.position = currentPosition;

				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}

        private Vector3 quadraticTrajectory(float normailzedTime) => new Vector3(normailzedTime - Mathf.Pow(normailzedTime, 2), 0f, 0f);

        private Vector3 ComputeMJTPosition(float normailzedTime) =>
			new (MJTEquation(initialPosition.x, finalPositionPhysical.x, normailzedTime),
				 0f,
				 MJTEquation(initialPosition.z, finalPositionPhysical.z, normailzedTime));

        private float MJTEquation(float initial, float final, float t) =>
			Mathf.LerpUnclamped(initial, final, 6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3));
    }
}
