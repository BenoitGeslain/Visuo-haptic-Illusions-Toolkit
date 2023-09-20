using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// To reset redirection, set the technique enum to None.
	/// </summary>
	public class WorldRedirection : Interaction {

		public WRTechnique technique;
		public WorldRedirectionTechnique techniqueInstance;
		public WRStrategy strategy;
		public WorldRedirectionStrategy strategyInstance;

		public WorldRedirectionScene scene;

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
				WRStrategy.None => new NoSteer(),
				WRStrategy.SteerToCenter => new SteerToCenter(),
				WRStrategy.SteerToOrbit => new SteerToOrbit(),
				WRStrategy.SteerToMultipleTargets => new SteerToMultipleTargets(),
				_ => null
			};

			if (strategyInstance is null)
				Debug.LogError("Error Unknown Redirection strategy.");
		}

		private void Start() {
			updateTechnique();

			scene.selectedTarget = scene.targets[0];
			scene.previousPosition = scene.physicalHead.position;
			scene.previousRotation = scene.physicalHead.rotation;
		}

		private void Update() {
			updateTechnique();

			if (techniqueInstance is not null && strategyInstance is not null) {
				scene.forwardTarget = strategyInstance.SteerTo(scene);
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

        public bool IsRedirecting() => scene.GetHeadToHeadDistance() < Vector3.kEpsilon &&
                   Quaternion.Angle(scene.physicalHead.rotation, scene.virtualHead.rotation) < Vector3.kEpsilon;
    }
}