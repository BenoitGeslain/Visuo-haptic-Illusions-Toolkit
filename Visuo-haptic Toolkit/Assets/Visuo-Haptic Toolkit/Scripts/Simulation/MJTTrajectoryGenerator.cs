using UnityEngine;

using BG.Redirection;

namespace BG.Trajectory {
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
			scene = ((BodyRedirection)Toolkit.Instance.rootScript).scene;

			initialPosition = scene.origin.position;
			currentPosition = scene.origin.position;
			finalPositionPhysical = scene.physicalTarget.position;
			finalPositionVirtual = scene.virtualTarget.position;

		}

		private void Update() {
			if (movementInProgress) {
				currentPosition = ComputeMJTPosition(currentTime / movementTime, currentTime / movementTime > ballisticAdjustementPhase);
				currentPosition.y = scene.physicalHand.position.y;
				scene.physicalHand.position = currentPosition;

				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}

		private Vector3 ComputeMJTPosition(float normailzedTime, bool virtualTarget) {
			float offset = (virtualTarget) ? (finalPositionPhysical.x - finalPositionVirtual.x) *
											 (normailzedTime - ballisticAdjustementPhase) * (ballisticAdjustementPhase / (1f - ballisticAdjustementPhase) + 1) : 0f;
			Debug.Log($"{normailzedTime} {offset} {normailzedTime - ballisticAdjustementPhase}");
			return new Vector3(MJTEquation(initialPosition.x, finalPositionVirtual.x, normailzedTime) + offset,
							   0f,
							   MJTEquation(initialPosition.z, finalPositionVirtual.z, normailzedTime));
		}

		private float MJTEquation(float initial, float final, float t) {
			return initial + (final - initial) * (6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3));
		}
	}
}
