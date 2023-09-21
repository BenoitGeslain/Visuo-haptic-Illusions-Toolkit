using System.Collections.Generic;

using UnityEngine;
using BG.Redirection;
using System;

namespace BG.Visualisation {
	public class WRVisuals : MonoBehaviour {

		private static List<Color> colors;	// colors of the lines between the physical and virtual elements

		public GameObject targetPrefab;
		private List<Transform> targets;

		// Calling Onenable instead of Start to support recompilation during play
		private void OnEnable() {
			colors = new List<Color> () {
				Color.white,	// Indicates the selected target
				Color.black,	// Indicates the other targets
				Color.green	// Indicates the forward target vector
			};
		}

		private void Start() {
			targets = new List<Transform>();
		}

		private void Update() {
			WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;
			WorldRedirectionScene scene = rootScript.scene;

			switch (rootScript.strategy) {
				case WRStrategy.None:
					if (targets.Count == 1) {
						targets[0].transform.position = scene.selectedTarget.position + scene.physicalHead.forward;
						targets[0].gameObject.SetActive(true);
					} else {
						for (int i = 0; i < targets.Count; i++) {
							Destroy(targets[i].gameObject);
						}
						targets.Clear();
						targets.Add(Instantiate(targetPrefab, this.transform).transform);
						targets[0].transform.position = scene.selectedTarget.position + scene.physicalHead.forward;
						targets[0].gameObject.SetActive(true);
					}
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

		private void drawDirectionLines(WorldRedirectionScene scene) {
			Debug.DrawRay(scene.physicalHead.position, scene.forwardTarget.normalized, colors[2]);
		}

		private void centerTargets(WorldRedirectionScene scene) {
			if (targets.Count == 1) {
				targets[0].transform.position = scene.selectedTarget.position;
				targets[0].gameObject.SetActive(true);
			} else {
				for (int i = 0; i < targets.Count; i++) {
					Destroy(targets[i].gameObject);
				}
				targets.Clear();
				targets.Add(Instantiate(targetPrefab, this.transform).transform);
				targets[0].transform.position = scene.selectedTarget.position;
				targets[0].gameObject.SetActive(true);
			}
			Debug.DrawLine(scene.physicalHead.position, scene.selectedTarget.position, colors[0]);
		}

		private void orbitTargets(WorldRedirectionScene scene) {
			float distanceToTarget = scene.GetDistanceToTarget();
			Vector3 vectorToTarget = Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up).normalized;
			if (distanceToTarget < scene.radius) {
				Vector3 leftTarget = Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.PI/3, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(Mathf.Rad2Deg * Mathf.PI/3 * Mathf.Rad2Deg));
				Vector3 rightTarget = Quaternion.Euler(0f, - Mathf.Rad2Deg * Mathf.PI/3, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(Mathf.Rad2Deg * Mathf.PI/3 * Mathf.Rad2Deg));

				if (Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)) {
					Debug.DrawRay(scene.physicalHead.position, leftTarget, colors[0]);
					Debug.DrawRay(scene.physicalHead.position, rightTarget, colors[1]);
				} else {
					Debug.DrawRay(scene.physicalHead.position, leftTarget, colors[1]);
					Debug.DrawRay(scene.physicalHead.position, rightTarget, colors[0]);
				}
				updateTargetsOrbit(scene, leftTarget, rightTarget);
			} else {
				float angleToTargets = Mathf.Rad2Deg * Mathf.Asin(scene.radius / distanceToTarget);
				Vector3 leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(angleToTargets * Mathf.Deg2Rad));
				Vector3 rightTarget = Quaternion.Euler(0f, - angleToTargets, 0f) * vectorToTarget * Mathf.Abs(scene.radius / Mathf.Tan(angleToTargets * Mathf.Deg2Rad));

				if (Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)) {
					Debug.DrawRay(scene.physicalHead.position, leftTarget, colors[0]);
					Debug.DrawRay(scene.physicalHead.position, rightTarget, colors[1]);
				} else {
					Debug.DrawRay(scene.physicalHead.position, leftTarget, colors[1]);
					Debug.DrawRay(scene.physicalHead.position, rightTarget, colors[0]);
				}
				updateTargetsOrbit(scene, leftTarget, rightTarget);
			}
		}

		private void updateTargetsOrbit(WorldRedirectionScene scene, Vector3 leftTarget, Vector3 rightTarget) {
			if (targets.Count == 2) {
				targets[0].position = scene.physicalHead.position + leftTarget;
				targets[1].position = scene.physicalHead.position + rightTarget;
				targets[0].gameObject.SetActive(true);
				targets[1].gameObject.SetActive(true);
			} else {
				for (int i = 0; i < targets.Count; i++) {
					Destroy(targets[i].gameObject);
				}
				targets.Clear();
				targets.Add(Instantiate(targetPrefab, this.transform).transform);
				targets.Add(Instantiate(targetPrefab, this.transform).transform);
				targets[0].position = scene.physicalHead.position + leftTarget;
				targets[1].position = scene.physicalHead.position + rightTarget;
				targets[0].gameObject.SetActive(true);
				targets[1].gameObject.SetActive(true);
			}
		}

		private void multipleTargets(WorldRedirectionScene scene) {
			if (targets.Count == scene.targets.Length) {
				for (int i = 0; i < scene.targets.Length; i++) {
					targets[i].transform.position = scene.targets[i].position;
					targets[i].gameObject.SetActive(true);
					if (scene.targets[i] == scene.selectedTarget) {
						Debug.DrawLine(scene.physicalHead.position, targets[i].position, colors[0]);
					} else {
						Debug.DrawLine(scene.physicalHead.position, targets[i].position, colors[1]);
					}
				}
			} else {
				for (int i = 0; i < targets.Count; i++) {
					Destroy(targets[i].gameObject);
				}
				targets.Clear();
				for (int i = 0; i < scene.targets.Length; i++) {
					targets.Add(Instantiate(targetPrefab, this.transform).transform);
					targets[i].transform.position = scene.targets[i].position;
					targets[i].gameObject.SetActive(true);
					if (scene.targets[i] == scene.selectedTarget) {
						Debug.DrawLine(scene.physicalHead.position, targets[i].position, colors[0]);
					} else {
						Debug.DrawLine(scene.physicalHead.position, targets[i].position, colors[1]);
					}
				}
			}
		}
	}
}
