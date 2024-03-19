using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace VHToolkit.Redirection {
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
			float angleToTargets = (distanceToTarget < scene.radius ? Mathf.PI / 3 : Mathf.Asin(scene.radius / distanceToTarget)) * Mathf.Rad2Deg;
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
            // Equivalent code in later .Net versions:
            // Func<Transform, float> bearing = (Transform t) => Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
            // Transform target = scene.targets.Min(bearing);

            float smallestBearing = 360f;
			Transform target = null;
			foreach (Transform t in scene.targets) {
				float a = Vector3.Angle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
				// a *= (scene.physicalHead.position - t.position).sqrMagnitude;
				if (a < smallestBearing) {
					smallestBearing = a;
					target = t;
				}
			}
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
			Debug.DrawRay(scene.physicalHead.position, Vector3.Reflect(scene.targets[0].position, scene.physicalHead.right));
            return Vector3.Reflect(scene.targets[0].position, scene.physicalHead.right);
        }
    }

    public class ApfRedirection : WorldRedirectionStrategy {

        List<Collider> colliders;

        private Func<Vector3, float> RepulsiveFunction() {
            if (colliders==null)
                colliders = GameObject.FindGameObjectsWithTag("Obstacle").Select(o => o.GetComponent<Collider>()).ToList();
            return MathTools.RepulsivePotential3D(colliders);
        }

        public override Vector3 SteerTo(Scene scene) => ComputeGradient(scene);

        public Vector2 ComputeGradient(Scene scene) => Vector3.ProjectOnPlane(MathTools.Gradient3(
                                            RepulsiveFunction(),
                                            MathTools.ProjectToHorizontalPlane(scene.physicalHead.position)
                                        ), Vector3.up);

        // Can this be removed?
        // public static Vector2 ComputeGradient(Scene scene) => ComputeGradient(scene.physicalHead.position);

        // public static Vector2 ComputeGradient(Vector3 position) => MathTools.Gradient3(
        //                                                                 RepulsiveFunction(),
        //                                                                 MathTools.ProjectToHorizontalPlane(position)
        //                                                             ).ProjectToHorizontalPlane();
    }
}
