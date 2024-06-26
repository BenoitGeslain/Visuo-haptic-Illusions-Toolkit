using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Visualisation {
	public class WRVisuals : MonoBehaviour {

		private WorldRedirection WRMainScript;

		public GameObject targetPrefab;
		private List<Transform> targets;

		public Material activeMaterial, inactiveMaterial;

		[Range(3, 100)]
		[SerializeField] private int orbitResolution;

		// Calling OnEnable instead of Start to support recompilation during play
		private void Start() {
			WRMainScript = GetComponent<WorldRedirection>();
			targets = new List<Transform>();
		}

		private void LateUpdate() {
			Scene scene = WRMainScript.scene;

			if (scene.forwardTarget != null) {
				Debug.Log(scene.forwardTarget);
				Debug.DrawRay(scene.physicalHead.position, scene.forwardTarget.normalized, Color.Lerp(new Color(1f, 0, 0f, 0.05f), Color.red, scene.forwardTarget.magnitude));
			}

			switch (WRMainScript.strategy) {
				case WRStrategy.NoSteering:
					FixTargetCounts(1);
					targets[0].position = scene.physicalHead.position + scene.physicalHead.forward;
					targets[0].gameObject.SetActive(true);
					break;
				case WRStrategy.SteerToCenter:
					CenterTarget(scene);
					break;
				case WRStrategy.SteerToOrbit:
					ShowOrbit(scene);
					ShowOrbitTargets(scene);
					break;
				case WRStrategy.SteerToMultipleTargets:
					MultipleTargets(scene);
					break;
				case WRStrategy.SteerInDirection:
					SteerInDirection(scene);
					break;
				case WRStrategy.Thomas2019APF_PushPull:
					(WRMainScript.strategyInstance as Thomas2019APF_PushPull).colliders.ForEach(o => Debug.DrawLine(scene.physicalHead.position, o.ClosestPoint(scene.physicalHead.position), Color.Lerp(Color.clear, Color.yellow, 1 / Vector3.Distance(scene.physicalHead.position, o.ClosestPoint(scene.physicalHead.position)))));
					// (WRMainScript.strategyInstance as APFP2R).colliders.ForEach(o => {
					// 	Debug.Log($"{o.gameObject.name} : {1 / Vector3.Distance(scene.physicalHead.position, o.ClosestPoint(scene.physicalHead.position))}");
					// });
					// Debug.Log(Vector3.Angle(Vector3.forward, scene.forwardTarget));
					break;
				default:
					targets.Clear();
					break;
			}
			targets.ForEach(t => t.gameObject.SetActive(true));
		}

		private void CenterTarget(Scene scene) {
			FixTargetCounts(1);
			targets[0].position = scene.targets[0].position;
			targets[0].GetComponent<Renderer>().material = activeMaterial;
			Debug.DrawLine(scene.physicalHead.position, scene.targets[0].position, Color.blue);
		}

		private void ShowOrbit(Scene scene) {
			var firstTarget = scene.targets.FirstOrDefault();
			if (firstTarget == null) {
				return;
			}
			Vector3 previousRadius = new(0f, 0f, scene.parameters.SteerToOrbitRadius);
			Quaternion stepRotation = Quaternion.Euler(0f, 360f / orbitResolution, 0f);

			for (int angle = 0; angle < orbitResolution; angle++) {
				Vector3 currentRadius = stepRotation * previousRadius;

				Vector3 start = firstTarget.position + previousRadius;
				Vector3 end = firstTarget.position + currentRadius;

				start.y = end.y = scene.physicalHead.position.y;

				Debug.DrawLine(start, end);
				previousRadius = currentRadius;
			}
		}

		private void ShowOrbitTargets(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			Vector3 vectorToTarget = Vector3.ProjectOnPlane(scene.targets[0].position - scene.physicalHead.position, Vector3.up);
			Vector3 leftTarget, rightTarget;

			if (distanceToTarget < scene.parameters.SteerToOrbitRadius) {
				var length = 0.5f * (distanceToTarget + Mathf.Sqrt(4 * Mathf.Pow(scene.parameters.SteerToOrbitRadius, 2f) - 3 * Mathf.Pow(distanceToTarget, 2f)));
				leftTarget = Quaternion.Euler(0f, 60f, 0f) * (length * vectorToTarget.normalized);
				rightTarget = Quaternion.Euler(0f, -60f, 0f) * (length * vectorToTarget.normalized);
			}
			else {
				float angleToTargetsInRadians = Mathf.Asin(scene.parameters.SteerToOrbitRadius / distanceToTarget);
				float angleToTargetsInDegrees = angleToTargetsInRadians * Mathf.Rad2Deg;

				leftTarget = Quaternion.Euler(0f, angleToTargetsInDegrees, 0f) * vectorToTarget * Mathf.Cos(angleToTargetsInRadians);
				rightTarget = Quaternion.Euler(0f, -angleToTargetsInDegrees, 0f) * vectorToTarget * Mathf.Cos(angleToTargetsInRadians);
			}

			UpdateTargetsOrbit(scene, leftTarget, rightTarget);
		}

		private void UpdateTargetsOrbit(Scene scene, Vector3 leftTarget, Vector3 rightTarget) {
			FixTargetCounts(2);
			bool leftTargetIsActive = Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget);
			int activeTargetIndex = leftTargetIsActive ? 0 : 1;
			targets[activeTargetIndex].GetComponent<Renderer>().material = activeMaterial;
			targets[1 - activeTargetIndex].GetComponent<Renderer>().material = inactiveMaterial;
			Debug.DrawRay(scene.physicalHead.position, leftTargetIsActive ? leftTarget : rightTarget, Color.blue);

			targets[0].position = scene.physicalHead.position + leftTarget;
			targets[1].position = scene.physicalHead.position + rightTarget;
		}

		private void MultipleTargets(Scene scene) {
			FixTargetCounts(scene.targets.Count(x => x != null));

			foreach (var (first, second) in targets.Zip(scene.targets.Where(x => x != null))) {
				first.transform.position = second.position;

				if (second == scene.selectedTarget) {
					Debug.DrawLine(scene.physicalHead.position, first.position, Color.blue);
					first.GetComponent<Renderer>().material = activeMaterial;
				}
				else {
					first.GetComponent<Renderer>().material = inactiveMaterial;
				}
			}
		}

		private void FixTargetCounts(int count) {
			if (targets.Count != count) {
				targets.ForEach(t => Destroy(t.gameObject));
				targets.Clear();
				targets.AddRange(
					Enumerable.Range(0, count).Select(_ => Instantiate(targetPrefab, transform).transform)
				);
			}
			Debug.Assert(targets.Count == count);
		}

		private void SteerInDirection(Scene scene) => Debug.DrawRay(scene.physicalHead.position, scene.physicalHead.rotation * scene.strategyDirection);
	}
}
