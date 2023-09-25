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

		private void Start() {
			updateTechnique();
			scene.previousPosition = scene.physicalHead.position;
			scene.previousRotation = scene.physicalHead.rotation;
		}

        private void Update() {
			updateTechnique();
			techniqueInstance?.Redirect(scene);

			scene.virtualHand.rotation = scene.physicalHand.rotation;
			scene.previousPosition = scene.physicalHead.position;
			scene.previousRotation = scene.physicalHead.rotation;
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