using System;
using UnityEngine;

namespace BG.Redirection {
	[Serializable]
	public class WorldRedirectionStrategy {

		public Transform[] targets;
		public float radius;

        public WorldRedirectionStrategy(Transform[] targets, float radius) {
			this.targets = targets;
			this.radius = radius;
		}

		public virtual Vector3 SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	// Incomplete implementation
	public class SteerToCenter: WorldRedirectionStrategy {

		// public SteerToCenter(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToCenter(Transform[] targets, float radius): base(targets, radius) {}

		public override Vector3 SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.Log("Forwards: " + targets[0].position + "/" + physicalHead.position);
			return targets[0].position - physicalHead.position;
		}
	}

	public class SteerToOrbit: WorldRedirectionStrategy {

		// public SteerToOrbit(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToOrbit(Transform[] targets, float radius): base(targets, radius) {}


		public override Vector3 SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}

	public class SteerToMultipleTargets: WorldRedirectionStrategy {

		// public SteerToMultipleTargets(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToMultipleTargets(Transform[] targets, float radius): base(targets, radius) {}

		public override Vector3 SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
			return Vector3.zero;
		}
	}
}
