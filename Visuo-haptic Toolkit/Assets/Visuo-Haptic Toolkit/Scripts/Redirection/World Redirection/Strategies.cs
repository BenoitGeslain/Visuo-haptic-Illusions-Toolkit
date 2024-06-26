using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace VHToolkit.Redirection.WorldRedirection {
	public abstract class WorldRedirectionStrategy {
		/// <summary>
		/// Abstract method for producing a direction in which the user ought to be steered.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public abstract Vector3 SteerTo(Scene scene);
	}

	public class NoSteering : WorldRedirectionStrategy {
		public override Vector3 SteerTo(Scene scene) => scene.physicalHead.forward;
	}

	public class SteerToCenter : WorldRedirectionStrategy {
		public override Vector3 SteerTo(Scene scene) => scene.targets[0].position - scene.physicalHead.position;
	}

	public class SteerToOrbit : WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) {
			float distanceToTarget = scene.GetHeadToTargetDistance();
			float angleToTargets = (distanceToTarget < scene.parameters.SteerToOrbitRadius ? Mathf.PI / 3 : Mathf.Asin(scene.parameters.SteerToOrbitRadius / distanceToTarget)) * Mathf.Rad2Deg;
			var v = Vector3.ProjectOnPlane(scene.targets[0].position - scene.physicalHead.position, Vector3.up);
			Vector3 leftTarget = Quaternion.Euler(0f, angleToTargets, 0f) * v;
			Vector3 rightTarget = Quaternion.Euler(0f, -angleToTargets, 0f) * v;
			return Vector3.Angle(leftTarget, scene.physicalHead.forward) < Vector3.Angle(scene.physicalHead.forward, rightTarget)
				? leftTarget
				: rightTarget;
		}
	}

	public class SteerToMultipleTargets : WorldRedirectionStrategy {
		/// <summary>
		/// Select the target that has the smallest bearing with the orientation of the user's head.
		/// Return a vector pointing from the physical head in the direction of the selected target.
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public override Vector3 SteerTo(Scene scene) {
			float bearing(Transform t) => Vector3.Angle(
				Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up),
				Vector3.ProjectOnPlane(t.position - scene.physicalHead.position, Vector3.up));
			scene.selectedTarget = scene.targets.Where(t => t != null).MinBy(bearing);
			if (scene.selectedTarget == null) {
				Debug.LogWarning("Using SteerToMultipleTargets but scene.targets is empty or all-null.");
				return scene.physicalHead.forward;
			}
			return scene.selectedTarget.position - scene.physicalHead.position;
		}
	}

	public class SteerInDirection : WorldRedirectionStrategy {
		/// <summary>
		/// Steers the user in the specified direction relative to the physicalHead orientation.
		/// This function uses the parameter strategyDirection as its input.
		/// For example, if strategyDirection is Vector3.right, this function will return physicalHead.right
		/// </summary>
		/// <param name="scene"></param>
		/// <returns></returns>
		public override Vector3 SteerTo(Scene scene) => scene.physicalHead.rotation * scene.strategyDirection;
	}

	public class Thomas2019APF_PushPull : WorldRedirectionStrategy {

		/// <value>A list of colliders, each of which will create a repulsive potential field.</value>
		public List<Collider> colliders;

		public Thomas2019APF_PushPull() {
			colliders = GameObject.FindGameObjectsWithTag("Obstacle").Select(o => o.GetComponent<Collider>()).ToList();
			// colliders.ForEach(o => o.transform.position = MathTools.ProjectToHorizontalPlaneV3(o.transform.position));
		}

		public override Vector3 SteerTo(Scene scene) => ComputeGradient(scene);

		private Vector3 ComputeGradient(Scene scene) => Vector3.ProjectOnPlane(MathTools.Gradient3(
											MathTools.RepulsivePotential3D(colliders),
											MathTools.ProjectToHorizontalPlaneV3(scene.physicalHead.position)
										), Vector3.up);
	}

	public class Messinger2019APF : WorldRedirectionStrategy {

		public override Vector3 SteerTo(Scene scene) => Vector3.forward;
	}
}
