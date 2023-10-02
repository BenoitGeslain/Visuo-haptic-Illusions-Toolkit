using System.Collections.Generic;

using UnityEngine;
using BG.Redirection;
using System;
using System.Linq;

namespace BG.Visualisation {
	public class WRVisuals : MonoBehaviour {

		private static List<Color> colors;	// colors of the lines between the physical and virtual elements

		public GameObject targetPrefab;
		private List<Transform> targets;

        // Calling OnEnable instead of Start to support recompilation during play
        private void OnEnable() => colors = new List<Color>() {
                Color.white,	// Indicates the selected target
				Color.black,	// Indicates the other targets
				Color.green	// Indicates the forward target vector
		};

        private void Start() => targets = new List<Transform>();

        private void Update() {
			WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;
			Scene scene = rootScript.scene;

			switch (rootScript.strategy) {
				case WRStrategy.None:
					fixTargetCounts(1);
                    targets[0].transform.position = scene.selectedTarget.position + scene.physicalHead.forward;
                    targets[0].gameObject.SetActive(true);
                    break;
				case WRStrategy.SteerToCenter:
					centerTargets(scene);
					break;
				case WRStrategy.SteerToOrbit:
					orbitTargets(scene);
					break;
				case WRStrategy.SteerToMultipleTargets:
					multipleTargets(scene);
					break;
				default:
					targets.Clear();
					break;
			}

			// draws threshold lines for the targets
			drawDirectionLines(scene);
		}

		private void drawDirectionLines(Scene scene) {
			Debug.DrawRay(scene.physicalHead.position, scene.forwardTarget.normalized, colors[2]);
		}

		private void centerTargets(Scene scene) {
			fixTargetCounts(1);
            targets[0].transform.position = scene.selectedTarget.position;
            targets[0].gameObject.SetActive(true);

            Debug.DrawLine(scene.physicalHead.position, scene.selectedTarget.position, colors[0]);
		}

		private void orbitTargets(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			Vector3 vectorToTarget = Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up).normalized;
			Vector3 leftTarget, rightTarget;
			if (distanceToTarget < scene.radius) {
				leftTarget = Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.PI/3, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(Mathf.Rad2Deg * Mathf.PI/3 * Mathf.Rad2Deg));
				rightTarget = Quaternion.Euler(0f, - Mathf.Rad2Deg * Mathf.PI/3, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(Mathf.Rad2Deg * Mathf.PI/3 * Mathf.Rad2Deg));
			} else {
				float angleToTargets = Mathf.Rad2Deg * Mathf.Asin(scene.radius / distanceToTarget);
				leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(angleToTargets * Mathf.Deg2Rad));
				rightTarget = Quaternion.Euler(0f, - angleToTargets, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(angleToTargets * Mathf.Deg2Rad));
			}
            var targetColors = (Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)) ?
				(colors[0], colors[1]) : (colors[1], colors[0]);

            Debug.DrawRay(scene.physicalHead.position, leftTarget, targetColors.Item1);
            Debug.DrawRay(scene.physicalHead.position, rightTarget, targetColors.Item2);
            updateTargetsOrbit(scene, leftTarget, rightTarget);
        }

		private void updateTargetsOrbit(Scene scene, Vector3 leftTarget, Vector3 rightTarget) {
			fixTargetCounts(2);
            targets[0].position = scene.physicalHead.position + leftTarget;
			targets[1].position = scene.physicalHead.position + rightTarget;
			targets.ForEach(t => t.gameObject.SetActive(true));
		}

		private void multipleTargets(Scene scene) {
			fixTargetCounts(scene.targets.Length);
            var targetsAndSceneTargets = targets.Zip(scene.targets, (a, b) => (a, b));
            foreach ((var first, var second) in targetsAndSceneTargets) {
                first.transform.position = second.position;
                first.gameObject.SetActive(true);
                Debug.DrawLine(scene.physicalHead.position, first.position,
                colors[(second == scene.selectedTarget) ? 0 : 1]);
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
