using UnityEngine;
using static UnityEngine.UI.Image;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// </summary>
	public class BodyRedirection: Interaction {

		public BRTechnique technique;
		public BodyRedirectionTechnique techniqueInstance;

        public BodyRedirectionScene scene;

		

        private void updateTechnique() {
			techniqueInstance = technique switch {
				BRTechnique.None => new ResetBodyRedirection(this),
				BRTechnique.Azmandian2016Body => new Azmandian2016Body(this),
				BRTechnique.Han2018Instant => new Han2018Instant(this),
				BRTechnique.Han2018Continous => new Han2018Continous(this),
				BRTechnique.Cheng2017Sparse => new Cheng2017Sparse(this),
				BRTechnique.Geslain2022Polynom => new Geslain2022Polynom(this, techniqueInstance.redirectionLateness, techniqueInstance.controlPoint),
				_ => null
			};

			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");
		}

		private void Start()
		{
			updateTechnique();
		}

        private void Update() {
			updateTechnique();
			techniqueInstance?.Redirect(scene);
			scene.virtualHand.rotation = scene.physicalHand.rotation;
		}

		public void SetTechnique(BRTechnique t) {
			technique = t;
			updateTechnique();
		}

        public void ResetRedirection() => SetTechnique(BRTechnique.None);

        public BRTechnique GetTechnique(BRTechnique t) => technique;

        public bool IsRedirecting() => scene.GetHandRedirectionDistance() > Vector3.kEpsilon;
    }
}