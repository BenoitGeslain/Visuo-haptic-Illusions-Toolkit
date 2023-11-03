using UnityEngine;

namespace VHToolkit.Redirection {
	/// <summary>
	/// This class allows users to select which body redirection technique to use as well as the relevant parameters.
	/// The enumeration BRTechnique allows for dynamic selection of the technique.
	/// The class reference BodyRedirectionTechnique contains the redirect function that is used to apply redirection with the Redirect() function.
	/// The scene parameter contains all the information necessary for the redirection such as the GameObjects representing the hand (physical and virtual) or the targets.
	///
	/// Every frame, the technique calls the Redirect() method of techniqueInstance.
	/// To add a new redirection function [TODO complete this section]
	/// </summary>
	public class BodyRedirection: Interaction {

        /// <summary>
        /// Currently selected technique, if any. When set, updateTechnique() gets called on the next Update().
        /// </summary>
        public BRTechnique Technique { set; get; }

        /// <summary>
        /// Previously selected technique, if any.
        /// </summary>
        private BRTechnique previousTechnique;

		[SerializeField] private BodyRedirectionTechnique techniqueInstance;

		/// <summary>
		/// Updates the techniqueInstance according to the enumeration technique chosen.
		/// </summary>
        private void updateTechnique() {
			techniqueInstance = Technique switch {
				BRTechnique.Reset => new ResetBodyRedirection(),
				BRTechnique.Azmandian2016Body => new Azmandian2016Body(),
				BRTechnique.Azmandian2016Hybrid => new Azmandian2016Hybrid(),
				BRTechnique.Han2018TranslationalShift => new Han2018Instant(),
				BRTechnique.Han2018InterpolatedReach => new Han2018Continuous(),
				BRTechnique.Cheng2017Sparse => new Cheng2017Sparse(),
				BRTechnique.Geslain2022Polynom => new Geslain2022Polynom(techniqueInstance.redirectionLateness, techniqueInstance.controlPoint),
				BRTechnique.Poupyrev1996GoGo => new Poupyrev1996GoGo(),
				BRTechnique.Lecuyer2000Swamp => new Lecuyer2000Swamp(),
				BRTechnique.Samad2019Weight => new Samad2019Weight(),
				_ => null
			};

			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");
		}

		/// <summary>
		/// Start function called once when the game is starting. This fucntion calls updateTechnique() to instantiate the technique class and
		/// initializes the previous head positions.
		/// </summary>
		private void OnEnable() {
			updateTechnique();
			previousTechnique = Technique;
			// Store thhe previous hand and head position and rotation to compute instant linear or angular velocity
			scene.previousHandPosition = scene.physicalHand.position;
			scene.previousHandRotation = scene.physicalHand.rotation;
			scene.previousHeadPosition = scene.physicalHead.position;
			scene.previousHeadRotation = scene.physicalHead.rotation;
		}

		/// <summary>
		/// Update function called once per frame. This function
		/// calls updateTechnique() to instantiate the technique class,
		/// calls Redirect(...) from the BodyRedirection class to apply the redirection,
		/// applies rotations to the physical hand and
		/// initializes the previous head positions.
		/// </summary>
        private void Update() {
			if (previousTechnique != Technique) {
				updateTechnique();
				previousTechnique = Technique;
			}

			techniqueInstance?.Redirect(scene);	// Computes and applies the redirection according to the selected redirection technique

			// Copy the real hand rotation to the virtual hand to conserve tracking.
			scene.virtualHand.rotation = scene.physicalHand.rotation;	// TODO check Azmandian Body for rotation
			// In case the body redirection technique uses the head of the user (e.g. ),
			// the previous position and rotation are stored to compute instant linear or angular velocity
			scene.previousHandPosition = scene.physicalHand.position;
			scene.previousHandRotation = scene.physicalHand.rotation;
			scene.previousHeadPosition = scene.physicalHead.position;
			scene.previousHeadRotation = scene.physicalHead.rotation;
		}

		/// <summary>
		/// A wrapper around SetTechnique(BRTechnique t) to use the ResetRedirection technique.
		/// </summary>
        public void ResetRedirection() => Technique = BRTechnique.Reset;

		/// <summary>
		/// Returns whether a redirection is applied to the user's virtual and physical hand
		/// </summary>
		/// <returns>Returns a bool:
		/// true if the virtual hand of the user is not co-localised to the physical hand.
		/// false otherwise.</returns>
        public bool IsRedirecting() => scene.GetHandRedirectionDistance() > Vector3.kEpsilon;
    }
}