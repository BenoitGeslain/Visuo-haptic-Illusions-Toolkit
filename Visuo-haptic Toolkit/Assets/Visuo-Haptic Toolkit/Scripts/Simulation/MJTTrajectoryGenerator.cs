using System.Linq;
using UnityEngine;

using VHToolkit.Redirection;

namespace VHToolkit.Simulation {
	public class MJTTrajectoryGenerator : MonoBehaviour {

		private Scene scene;

		float currentTime = 0f;
		float movementTime = 2f;

		public bool movementInProgress = false;
		[Range(0f, 1f)]
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
				currentPosition = 0.25f * QuadraticTrajectory(currentTime / movementTime) + ComputeMJTPosition(currentTime / movementTime);
				currentPosition.y = scene.limbs.First().physicalLimb.position.y;
				scene.limbs.First().physicalLimb.position = currentPosition;

				currentTime += Time.deltaTime;

				if (currentTime > movementTime) {
					currentTime = 0;
					movementInProgress = false;
				}
			}
		}

        private Vector3 QuadraticTrajectory(float normalizedTime) => (normalizedTime - Mathf.Pow(normalizedTime, 2)) * Vector3.right;

        private Vector3 ComputeMJTPosition(float normalizedTime) =>
			new (MJTEquation(initialPosition.x, finalPositionPhysical.x, normalizedTime),
				 0f,
				 MJTEquation(initialPosition.z, finalPositionPhysical.z, normalizedTime));

        private float MJTEquation(float initial, float final, float t) =>
			Mathf.LerpUnclamped(initial, final, 6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3));
    }
}
