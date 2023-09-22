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

        public Scene scene;



        private void updateTechnique() {
			techniqueInstance = technique switch {
				BRTechnique.None => new ResetBodyRedirection(),
				BRTechnique.Azmandian2016Body => new Azmandian2016Body(),
				BRTechnique.Han2018Instant => new Han2018Instant(),
				BRTechnique.Han2018Continous => new Han2018Continous(),
				BRTechnique.Cheng2017Sparse => new Cheng2017Sparse(),
				BRTechnique.Geslain2022Polynom => new Geslain2022Polynom(techniqueInstance.redirectionLateness, techniqueInstance.controlPoint),
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