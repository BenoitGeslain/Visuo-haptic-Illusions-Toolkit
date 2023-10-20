using UnityEngine;

namespace BG.Redirection {
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
		/// Currently selected technique, if any.
		/// </summary>
		public BRTechnique technique;

		/// <summary>
		/// Previously selected technique, if any.
		/// </summary>
		private BRTechnique previousTechnique;


		[SerializeField] private BodyRedirectionTechnique techniqueInstance;

        public Scene scene;

		/// <summary>
		/// Updates the techniqueInstance according to the enumeration technique chosen.
		/// </summary>
        private void updateTechnique() {
			techniqueInstance = technique switch {
				BRTechnique.None => new ResetBodyRedirection(),
				BRTechnique.Azmandian2016Body => new Azmandian2016Body(),
				BRTechnique.Azmandian2016Hybrid => new Azmandian2016Hybrid(),
				BRTechnique.Han2018Instant => new Han2018Instant(),
				BRTechnique.Han2018Continous => new Han2018Continous(),
				BRTechnique.Cheng2017Sparse => new Cheng2017Sparse(),
				BRTechnique.Geslain2022Polynom => new Geslain2022Polynom(techniqueInstance.redirectionLateness, techniqueInstance.controlPoint),
				BRTechnique.Poupyrev1996GoGo => new Poupyrev1996GoGo(),
				_ => null
			};

			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");
		}

		/// <summary>
		/// Start function called once when the game is starting. This fucntion calls updateTechnique() to instantiate the technique class and
		/// initializes the previous head positions.
		/// </summary>
		private void Start() {
			updateTechnique();
			previousTechnique = technique;
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
			if (previousTechnique != technique) {
				updateTechnique();
				previousTechnique = technique;
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
		/// Setter for the enumeration BRTechnique. updateTechnique() gets called on the next Update().
		/// </summary>
		/// <param name="t">The enumeration defining which technique to call Redirect(...) from.</param>
		public void SetTechnique(BRTechnique t) {
			technique = t;
		}

		/// <summary>
		/// A wrapper around SetTechnique(BRTechnique t) to use the ResetRedirection technique.
		/// </summary>
        public void ResetRedirection() => SetTechnique(BRTechnique.None);

		/// <summary>
		/// Getter for the enumeration technique.
		/// </summary>
		/// <returns>Returns the enumeration technique</returns>
        public BRTechnique GetTechnique() => technique;

		/// <summary>
		/// Returns whether a redirection is applied to the user's virtual and physical hand
		/// </summary>
		/// <returns>Returns a bool:
		/// true if the virtual hand of the user is not co-localised to the physical hand.
		/// false otherwise.</returns>
        public bool IsRedirecting() => scene.GetHandRedirectionDistance() > Vector3.kEpsilon;
    }
}