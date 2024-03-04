using System.Linq;
using UnityEngine;

namespace VHToolkit.Redirection.WorldRedirection {
	public class WorldRedirectionStrategy {

		public virtual Vector3 SteerTo(Scene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class NoSteering: WorldRedirectionStrategy {
        public override Vector3 SteerTo(Scene scene) => scene.physicalHead.forward;
    }

	public class SteerToCenter: WorldRedirectionStrategy {
        public override Vector3 SteerTo(Scene scene) => scene.targets[0].position - scene.physicalHead.position;
    }

	public class SteerToOrbit: WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			float angleToTargets = (distanceToTarget < scene.parameters.steerToOrbitRadius ? Mathf.PI / 3 : Mathf.Asin(scene.parameters.steerToOrbitRadius / distanceToTarget)) * Mathf.Rad2Deg;
			var v = Vector3.ProjectOnPlane(scene.targets[0].position - scene.physicalHead.position, Vector3.up);
            Vector3 leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * v;
			Vector3 rightTarget = Quaternion.Euler(0f, -angleToTargets, 0f) * v;
            return Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)
                ? leftTarget
                : rightTarget;
        }
    }

	public class SteerToMultipleTargets: WorldRedirectionStrategy {
		/// <summary>
		/// Select the target that has the smallest bearing with the orientation of the user's head.
		/// Return a vector pointing from the physical head in the direction of the selected target.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public override Vector3 SteerTo(Scene scene) {
            float bearing(Transform t) => Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
            Transform target = scene.targets.MinBy(bearing);
			scene.selectedTarget = target;
			return target.position - scene.physicalHead.position;
		}
	}

	class SteerInDirection: WorldRedirectionStrategy {
        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public override Vector3 SteerTo(Scene scene) {
			Debug.DrawRay(scene.physicalHead.position, scene.strategyDirection);
            return scene.strategyDirection;
        }
    }
}
