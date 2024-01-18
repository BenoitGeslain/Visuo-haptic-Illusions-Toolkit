using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace VHToolkit.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// To reset redirection, set the technique enum to None.
	/// </summary>
	public class WorldRedirection : Interaction {

		public WRTechnique technique;
        /// <summary>
        /// Previously selected technique, if any.
        /// </summary>
        private WRTechnique previousTechnique;
		public WorldRedirectionTechnique techniqueInstance;
		public WRStrategy strategy;
		public WorldRedirectionStrategy strategyInstance;

		/// <summary>
		/// Updates the techniqueInstance according to the enumeration technique chosen.
		/// </summary>
		private void updateTechnique() {
			techniqueInstance = technique switch {
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
				_ => null
			};

			if (strategyInstance is null)
				Debug.LogError("Error Unknown Redirection strategy.");
		}

		/// <summary>
		/// Start function called once when the game is starting. This function calls updateTechnique() to instantiate the technique class and
		/// initializes the previous head positions.
		/// </summary>
		private void Start() {
			updateTechnique();
			previousTechnique = technique;

			if (scene.targets != null) {
				scene.selectedTarget = scene.targets.FirstOrDefault();
			}
			scene.previousHeadPosition = scene.physicalHead.position;
			scene.previousHeadRotation = scene.physicalHead.rotation;

			scene.previousLimbPositions = scene.limbs.Select(limb => limb.PhysicalLimb.position).ToList();
		}

		/// <summary>
		/// Update function called once per frame. This function
		/// calls updateTechnique() to instantiate the technique class,
		/// calls Redirect(...) from the BodyRedirection class to apply the redirection,
		/// applies rotations to the physical hand and
		/// initializes the previous head positions.
		/// </summary>
		private void Update() {
			if (previousTechnique != technique || techniqueInstance == null) {
				updateTechnique();
				previousTechnique = technique;
			}

			if (strategyInstance is not null)
				scene.forwardTarget = strategyInstance.SteerTo(scene);

			if (!redirect)
				new NoWorldRedirection().Redirect(scene);
			else if (techniqueInstance is not null)
				techniqueInstance.Redirect(scene);
			scene.previousHeadPosition = scene.physicalHead.position;
			scene.previousHeadRotation = scene.physicalHead.rotation;

			scene.previousLimbPositions = scene.limbs.Select(limb => limb.PhysicalLimb.position).ToList();
		}

		/// <summary>
		/// Getter for the enumeration technique.
		/// </summary>
		/// <returns>Returns the enumeration technique</returns>
		public WRTechnique GetTechnique() => technique;

        /// <summary>
        /// Setter for the enumeration BRTechnique. updateTechnique() gets called on the next Update().
        /// </summary>
        /// <param name="t">The enumeration defining which technique to call Redirect(...) from.</param>
        public void SetTechnique(WRTechnique t) => technique = t;

        /// <summary>
        /// A wrapper around SetTechnique(BRTechnique t) to use the ResetRedirection technique.
        /// </summary>
        public void ResetRedirection() => SetTechnique(WRTechnique.Reset);

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