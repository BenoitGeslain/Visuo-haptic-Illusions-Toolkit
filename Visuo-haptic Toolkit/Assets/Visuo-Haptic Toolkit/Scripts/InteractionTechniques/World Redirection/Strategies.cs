using System;
using UnityEngine;

namespace BG.Redirection {
	public class WorldRedirectionStrategy {

		public virtual Vector3 SteerTo(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class NoSteer: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			return scene.physicalHead.forward;
		}
	}

	public class SteerToCenter: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			return Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
		}
	}

	public class SteerToOrbit: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			float distanceToTarget = scene.GetDistanceToTarget();
			if (distanceToTarget < scene.radius) {
				Vector3 leftTarget = Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.PI/3, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
				Vector3 rightTarget = Quaternion.Euler(0f, - Mathf.Rad2Deg * Mathf.PI/3, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);

				Debug.DrawRay(scene.physicalHead.position, leftTarget, Color.red);
				Debug.DrawRay(scene.physicalHead.position, rightTarget, Color.green);

				if (Vector3.Angle(leftTarget, scene.physicalHead.forward) > Vector3.Angle(scene.physicalHead.forward, rightTarget)) {
					return leftTarget;
				}
				return rightTarget;
			} else {
				float angleToTargets = Mathf.Rad2Deg * Mathf.Asin(scene.radius / distanceToTarget);
				Vector3 leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
				Vector3 rightTarget = Quaternion.Euler(0f, - angleToTargets, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);

				Debug.DrawRay(scene.physicalHead.position, leftTarget, Color.red);
				Debug.DrawRay(scene.physicalHead.position, rightTarget, Color.green);

				if (Vector3.Angle(leftTarget, scene.physicalHead.forward) > Vector3.Angle(scene.physicalHead.forward, rightTarget)) {
					return leftTarget;
				}
				return rightTarget;
			}
		}
	}

	public class SteerToMultipleTargets: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			float smallestBearing = 360f;
			Transform target = scene.physicalHead;
			foreach (Transform t in scene.targets) {
				float a = Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(scene.forwardTarget, Vector3.up));
				if (a < smallestBearing) {
					smallestBearing = a;
					target = t;
				}
			}
			scene.selectedTarget = target;
			return target.position;
		}
	}
}
