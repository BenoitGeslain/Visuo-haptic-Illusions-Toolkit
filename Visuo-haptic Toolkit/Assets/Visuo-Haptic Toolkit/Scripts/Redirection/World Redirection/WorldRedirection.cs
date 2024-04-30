using System.Collections.Generic;

using UnityEngine;

namespace VHToolkit.Redirection.WorldRedirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// To reset redirection, set the technique enum to None.
	/// </summary>
	public class WorldRedirection : Interaction {

		[SerializeField] private WRTechnique _technique;
		public WRTechnique Technique { get => _technique; set => _technique = value; }
		private WRTechnique previousTechnique;
		public WorldRedirectionTechnique techniqueInstance;
		public WRStrategy strategy;
		public WorldRedirectionStrategy strategyInstance;

		/// <summary>
		/// Updates the techniqueInstance according to the enumeration technique chosen.
		/// </summary>
		private void UpdateTechnique() {
			techniqueInstance = _technique switch {
				WRTechnique.None => new NoWorldRedirection(),
				WRTechnique.Reset => new ResetWorldRedirection(),
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
				WRStrategy.NoSteering => new NoSteering(),
				WRStrategy.SteerToCenter => new SteerToCenter(),
				WRStrategy.SteerToOrbit => new SteerToOrbit(),
				WRStrategy.SteerToMultipleTargets => new SteerToMultipleTargets(),
				WRStrategy.SteerInDirection => new SteerInDirection(),
				WRStrategy.APF_PushPull => new APFP2R(), // Still to implement TODO
				_ => null
			};

			if (strategyInstance is null)
				Debug.LogError("Error Unknown Redirection strategy.");
		}

		/// <summary>
		/// Start function called once when the game is starting. This function calls updateTechnique() to instantiate the technique class and
		/// initializes the previous head positions.
		/// </summary>
		private void OnEnable() {
			UpdateTechnique();
			previousTechnique = Technique;

			scene.previousHeadPosition = scene.physicalHead.position;
			scene.previousHeadRotation = scene.physicalHead.rotation;

			scene.previousLimbPositions = scene.limbs.ConvertAll(limb => limb.physicalLimb.position);
		}

		/// <summary>
		/// Update function called once per frame. This function
		/// calls updateTechnique() to instantiate the technique class,
		/// calls Redirect(...) from the WorldRedirection class to apply the redirection,
		/// applies redirection to the physical head.
		/// </summary>
		private void LateUpdate() {
			if (previousTechnique != Technique || techniqueInstance == null) {
				UpdateTechnique();
				previousTechnique = Technique;
			}

			if (!redirect)
				new NoWorldRedirection().Redirect(scene);
			else if (techniqueInstance is not null) {
				if (strategyInstance is not null)
					scene.forwardTarget = strategyInstance.SteerTo(scene);
				techniqueInstance.Redirect(scene);
			}
			scene.physicalHead.GetPositionAndRotation(out scene.previousHeadPosition, out scene.previousHeadRotation);
			scene.previousLimbPositions = scene.limbs.ConvertAll(limb => limb.physicalLimb.position);
		}

		/// <summary>
		/// A wrapper around SetTechnique(BRTechnique t) to use the ResetRedirection technique.
		/// </summary>
		public void ResetRedirection() => Technique = WRTechnique.Reset;

		public Quaternion GetAngularRedirection() => scene.HeadToHeadRedirection;

		public float GetTranslationalRedirection() => scene.GetHeadToHeadDistance();    // TODO : maybe should be a vector 3 between heads instead of magn

		public void SetTargets(List<Transform> targets) => scene.targets = targets;

		public List<Transform> GetTargets() => scene.targets;

		public void SetApplyDampening(bool dampening) => scene.applyDampening = dampening;

		public bool GetApplyDampening() => scene.applyDampening;

		public void SetApplySmoothing(bool smoothing) => scene.applySmoothing = smoothing;

		public bool GetApplySmoothing() => scene.applySmoothing;

		/// <summary>
		/// Determine whether a redirection is applied to the user's virtual and physical head
		/// </summary>
		/// <returns>Returns a bool:
		/// - true if the virtual hand of the user is not co-localised to the physical head.
		/// - false otherwise.</returns>
		public bool IsRedirecting() => scene.GetHeadToHeadDistance() < Vector3.kEpsilon &&
				   Quaternion.Dot(scene.physicalHead.rotation, scene.virtualHead.rotation) > 1 - Vector3.kEpsilon;
	}
}