using System;
using UnityEngine;

namespace BG.Redirection {
	[Serializable]
	public class WorldRedirectionStrategy {

		[SerializeField] private Vector3[] targets;
		[SerializeField] private float radius;

        // public WorldRedirectionStrategy(Vector3[] targets, float radius) {
        public WorldRedirectionStrategy() {}

		public virtual void SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class SteerToCenter: WorldRedirectionStrategy {

		// public SteerToCenter(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToCenter(): base() {}

		public override void SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class SteerToOrbit: WorldRedirectionStrategy {

		// public SteerToOrbit(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToOrbit(): base() {}


		public override void SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class SteerToMultipleTargets: WorldRedirectionStrategy {

		// public SteerToMultipleTargets(Vector3[] targets, float radius): base(targets, radius) {}
		public SteerToMultipleTargets(): base() {}

		public override void SteerTo(Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}
}
