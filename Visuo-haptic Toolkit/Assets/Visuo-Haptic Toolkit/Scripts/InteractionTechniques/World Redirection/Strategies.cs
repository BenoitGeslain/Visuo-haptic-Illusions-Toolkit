using System;
using UnityEngine;

namespace BG.Redirection {
	public class WorldRedirectionStrategy {

		public virtual Vector3 SteerTo(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	// Incomplete implementation
	public class SteerToCenter: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			return Vector3.ProjectOnPlane(scene.targets[0].position - scene.physicalHead.position, Vector3.up);
		}
	}

	public class SteerToOrbit: WorldRedirectionStrategy {


		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class SteerToMultipleTargets: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class SteerToMultipleTargetsCenter: WorldRedirectionStrategy {

		public override Vector3 SteerTo(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}
}
