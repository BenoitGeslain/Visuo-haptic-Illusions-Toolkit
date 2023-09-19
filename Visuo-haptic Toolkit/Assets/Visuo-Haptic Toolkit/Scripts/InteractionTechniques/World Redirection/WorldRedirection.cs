using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// To reset redirection, set the technique enum to None.
	/// </summary>
	public class WorldRedirection : Interaction {

		[SerializeField] public WRTechnique technique;
		[SerializeField] public WorldRedirectionTechnique techniqueInstance;

		[Header("User Parameters")]
		public Transform physicalHead;
		public Transform virtualHead;
		private Quaternion previousOrientation;
		private Vector3 previousPosition;

		public WorldRedirectionScene scene;

		[Header("Technique Parameters")]
		public WRStrategy strategy;
		public WorldRedirectionStrategy strategyInstance;

		private void updateTechnique() {
			techniqueInstance = technique switch {
				WRTechnique.None => new ResetWorldRedirection(),
				WRTechnique.Razzaque2001OverTimeRotation => new Razzaque2001OverTimeRotation(),
				WRTechnique.Steinicke2008Translational => new Steinicke2008Translational(),
				WRTechnique.Razzaque2001Rotational => new Razzaque2001Rotational(),
				WRTechnique.Razzaque2001Curvature => new Razzaque2001Curvature(),
				WRTechnique.Razzaque2001Hybrid => new Razzaque2001Hybrid(),
				WRTechnique.Azmandian2016World => new Azmandian2016World(),
				_ => null
			};

			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");

			strategyInstance = strategy switch {
				WRStrategy.SteerToCenter => new SteerToCenter(strategyInstance.targets, strategyInstance.radius),
				WRStrategy.SteerToOrbit => new SteerToOrbit(strategyInstance.targets, strategyInstance.radius),
				WRStrategy.SteerToMultipleTargets => new SteerToMultipleTargets(strategyInstance.targets, strategyInstance.radius),
				_ => null
			};

			if (strategyInstance is null)
				Debug.LogError("Error Unknown Redirection strategy.");
		}

		private void Start() {
			scene = new WorldRedirectionScene(physicalHead: physicalHead, virtualHead: virtualHead, forwardTarget: Vector3.one);
			updateTechnique();
		}

		private void Update() {
			updateTechnique();

			if (techniqueInstance is not null && strategyInstance is not null) {
				scene.forwardTarget = strategyInstance.SteerTo(scene.physicalHead, scene.virtualHead);
				techniqueInstance.Redirect(scene);
			}

			scene.previousPosition = scene.physicalHead.position;
			scene.previousRotation = scene.physicalHead.rotation;
		}

		public void SetTechnique(WRTechnique t) {
			technique = t;
			updateTechnique();
		}

        public void ResetRedirection() => SetTechnique(WRTechnique.None);

		public WRTechnique GetTechnique() => technique;

		public bool IsRedirecting() {
			return scene.GetHeadToHeadDistance() < Vector3.kEpsilon &&
				   Quaternion.Angle(scene.physicalHead.rotation, scene.virtualHead.rotation) < Vector3.kEpsilon;
		}
	}
}