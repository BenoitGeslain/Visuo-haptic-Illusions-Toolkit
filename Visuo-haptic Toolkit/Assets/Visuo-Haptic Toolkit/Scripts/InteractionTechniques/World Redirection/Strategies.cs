using System;
using System.Linq;
using UnityEngine;

namespace BG.Redirection {
	public class WorldRedirectionStrategy {

		public virtual Vector3 SteerTo(Scene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class NoSteer: WorldRedirectionStrategy {

        public override Vector3 SteerTo(Scene scene) => scene.physicalHead.forward;
    }

	public class SteerToCenter: WorldRedirectionStrategy {

        public override Vector3 SteerTo(Scene scene) => scene.selectedTarget.position - scene.physicalHead.position;
    }

	public class SteerToOrbit: WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) {
			float distanceToTarget = scene.GetHeadRedirectionDistance();
			Vector3 leftTarget, rightTarget;
			if (distanceToTarget < scene.radius) {
				leftTarget = Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.PI / 3, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
				rightTarget = Quaternion.Euler(0f, -Mathf.Rad2Deg * Mathf.PI / 3, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
			}
			else {
				float angleToTargets = Mathf.Rad2Deg * Mathf.Asin(scene.radius / distanceToTarget);
				leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
				rightTarget = Quaternion.Euler(0f, -angleToTargets, 0f) * Vector3.ProjectOnPlane(scene.selectedTarget.position - scene.physicalHead.position, Vector3.up);
			}
            return Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)
                ? leftTarget
                : rightTarget;
        }
    }

	public class SteerToMultipleTargets: WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) {
			float smallestBearing = 360f;
			Transform target = null;
			foreach (Transform t in scene.targets) {
				float a = Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
				if (a < smallestBearing) {
					smallestBearing = a;
					target = t;
				}
			}
			scene.selectedTarget = target;
			return target.position - scene.physicalHead.position;
		}
	}
}
