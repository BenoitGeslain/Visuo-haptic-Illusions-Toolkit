using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// </summary>
	public class BodyRedirection: Interaction{

		public BRTechnique technique;
		private BRTechnique previousTechnique;
		public BodyRedirectionTechnique techniqueInstance;

		[Header("User Parameters")]
		public Transform physicalHand;
		public Transform virtualHand;

		[Header("Technique Parameters")]
		public Transform origin;
		public Transform physicalTarget;
		public Transform virtualTarget;

		private void init() {
			techniqueInstance = technique switch {
				BRTechnique.None => new ResetRedirection(this),
				BRTechnique.Azmandian2016Body => new Azmandian2016Body(this),
				BRTechnique.Han2018Instant => new Han2018Instant(this),
				BRTechnique.Han2018Continous => new Han2018Continous(this),
				BRTechnique.Cheng2017Sparse => new Cheng2017Sparse(this),
				BRTechnique.Geslain2022Polynom => new Geslain2022Polynom(this),
				_ => null
			};
			if (techniqueInstance is null)
				Debug.LogError("Error Unknown Redirection technique.");

			previousTechnique = technique;
		}

        private void Start() => init();

        private void Update() {
			if (previousTechnique != technique) {
				init();
			}
			techniqueInstance?.Redirect(physicalTarget, virtualTarget, origin, physicalHand, virtualHand);
		}

		public void setTechnique(BRTechnique t) {
			technique = t;
			init();
		}
        public BRTechnique getTechnique(BRTechnique t) => technique;

        public bool IsRedirecting() => Vector3.Distance(physicalHand.position, virtualHand.position) > Vector3.kEpsilon;

        public void resetRedirection() => setTechnique(BRTechnique.None);
    }
}