using System.Collections.Generic;

using UnityEngine;
using VHToolkit.Redirection;
using System.Linq;

namespace VHToolkit.Visualisation {
	public class WRVisuals : MonoBehaviour {

		private static List<Color> colors;	// colors of the lines between the physical and virtual elements

		private WorldRedirection WRMainScript;

		public GameObject targetPrefab;
		private List<Transform> targets;

		public Material active, inactive;

		[Range(3, 100)]
		[SerializeField] private int orbitResolution;

        // Calling OnEnable instead of Start to support recompilation during play
        private void OnEnable() {
			colors = new List<Color>() {
                Color.white,	// Indicates the selected target
				Color.black,	// Indicates the other targets
				Color.green	// Indicates the forward target vector
			};
			WRMainScript = GetComponent<WorldRedirection>();
		}

        private void Start() => targets = new List<Transform>();

        private void Update() {
			Scene scene = WRMainScript.scene;

			switch (WRMainScript.strategy) {
				case WRStrategy.NoSteering:
					fixTargetCounts(1);
                    targets[0].position = scene.physicalHead.position + scene.physicalHead.forward;
                    targets[0].gameObject.SetActive(true);
                    break;
				case WRStrategy.SteerToCenter:
					centerTarget(scene);
					break;
				case WRStrategy.SteerToOrbit:
					showOrbit(scene);
					orbitTargets(scene);
					break;
				case WRStrategy.SteerToMultipleTargets:
                    multipleTargets(scene);
					break;
				default:
					targets.Clear();
					break;
			}
            targets.ForEach(t => t.gameObject.SetActive(true));
		}

		private void centerTarget(Scene scene) {
			fixTargetCounts(1);
            targets[0].position = scene.selectedTarget.position;
            targets[0].GetComponent<Renderer>().material = active;
			Debug.DrawLine(scene.physicalHead.position, scene.selectedTarget.position, colors[2]);
		}

		private void orbitTargets(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			Vector3 vectorToTarget = Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
			Vector3 leftTarget, rightTarget;
			if (distanceToTarget < scene.radius) {
				var length = 0.5f * (distanceToTarget + Mathf.Sqrt(4 * Mathf.Pow(scene.radius, 2f) - 3 * Mathf.Pow(distanceToTarget, 2f)));
				leftTarget =  Quaternion.Euler(0f, 60, 0f) * (length * vectorToTarget.normalized);
                rightTarget = Quaternion.Euler(0f, -60, 0f) * (length * vectorToTarget.normalized);
			} else {
				float angleToTargetsInRadians = Mathf.Asin(scene.radius / distanceToTarget);
				float angleToTargetsInDegrees = angleToTargetsInRadians * Mathf.Rad2Deg;
				leftTarget = Quaternion.Euler(0f, angleToTargetsInDegrees, 0f) * vectorToTarget * Mathf.Cos(angleToTargetsInRadians);
				rightTarget = Quaternion.Euler(0f, -angleToTargetsInDegrees, 0f) * vectorToTarget * Mathf.Cos(angleToTargetsInRadians);
			}
            var targetColors = (Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)) ?
				(colors[0], colors[1]) : (colors[1], colors[0]);

            updateTargetsOrbit(scene, leftTarget, rightTarget);
        }

		private void updateTargetsOrbit(Scene scene, Vector3 leftTarget, Vector3 rightTarget) {
			fixTargetCounts(2);
			if (Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)) {
            	targets[0].GetComponent<Renderer>().material = active;
            	targets[1].GetComponent<Renderer>().material = inactive;
				Debug.DrawRay(scene.physicalHead.position, leftTarget, colors[2]);
			} else {
            	targets[0].GetComponent<Renderer>().material = inactive;
            	targets[1].GetComponent<Renderer>().material = active;
				Debug.DrawRay(scene.physicalHead.position, rightTarget, colors[2]);
			}
            targets[0].position = scene.physicalHead.position + leftTarget;
			targets[1].position = scene.physicalHead.position + rightTarget;
		}

		private void showOrbit(Scene scene) {
			Vector3 previousRadius = new (0f, 0f, scene.radius);
			Vector3 currentRadius;
			Quaternion stepRotation = Quaternion.Euler(0f, 360f/orbitResolution, 0f);
			for (int angle = 0; angle < orbitResolution; angle++) {
				currentRadius = stepRotation * previousRadius;
				Vector3 start = scene.selectedTarget.position + previousRadius;
				start.y = scene.physicalHead.position.y;
				Vector3 end = scene.selectedTarget.position + currentRadius;
				end.y = scene.physicalHead.position.y;
				Debug.DrawLine(start, end);
				previousRadius = currentRadius;
			}
		}

		private void multipleTargets(Scene scene) {
			fixTargetCounts(scene.targets.Length);
            var targetsAndSceneTargets = targets.Zip(scene.targets, (a, b) => (a, b));
            foreach ((var first, var second) in targetsAndSceneTargets) {
                first.transform.position = second.position;
				if (second == scene.selectedTarget) {
                	Debug.DrawLine(scene.physicalHead.position, first.position, colors[2]);
            		first.GetComponent<Renderer>().material = active;
				} else {
            		first.GetComponent<Renderer>().material = inactive;
				}
            }
        }

		private void fixTargetCounts(int count) {
			if (targets.Count != count) {
                targets.ForEach(t => Destroy(t.gameObject));
                targets.Clear();
                targets.AddRange(
                    Enumerable.Range(0, count).Select(_ => Instantiate(targetPrefab, this.transform).transform)
                );
            }
			Debug.Assert(targets.Count == count);
		}
	}
}
