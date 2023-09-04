using UnityEngine;

namespace BG.Redirection {

	public enum BRTechnique {
		None,
		Han2018Instant,
		Han2018Continous,
		Azmandian2016Body,
		Azmandian2016World,
		Azmandian2016Hybrid,
		Cheng2017Sparse
	}

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// </summary>
	public class BodyRedirection: Interaction{

		public BRTechnique technique;
		private BRTechnique previousTechnique;
		public BodyRedirectionTechnique techniqueClass;

		[Header("User Parameters")]
		public Transform physicalHand;
		public Transform virtualHand;

		[Header("Technique Parameters")]
		public Transform origin;
		public Transform physicalTarget;
		public Transform virtualTarget;


		private void init() {
			switch (technique) {
				case BRTechnique.None:
					techniqueClass = new ResetRedirection(this);
					break;
				case BRTechnique.Azmandian2016Body:
					techniqueClass = new Azmandian2016Body(this);
					break;
				case BRTechnique.Azmandian2016World:
					techniqueClass = new Azmandian2016World(this);
					break;
				case BRTechnique.Azmandian2016Hybrid:
					techniqueClass = new Azmandian2016Hybrid(this);
					break;
				case BRTechnique.Han2018Instant:
					techniqueClass = new Han2018Instant(this);
					break;
				case BRTechnique.Han2018Continous:
					techniqueClass = new Han2018Continous(this);
					break;
				case BRTechnique.Cheng2017Sparse:
					techniqueClass = new Cheng2017Sparse(this);
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					break;
			}
			previousTechnique = technique;
		}

		private void Start() {
			init();
		}

		private void Update() {
			if (previousTechnique!=technique) {
				init();
			}
			techniqueClass.Redirect(physicalTarget, virtualTarget, origin, physicalHand, virtualHand);
		}

		public void setTechnique(BRTechnique t) {
			technique = t;
			init();
		}
		public BRTechnique getTechnique(BRTechnique t) {
			return technique;
		}

		public bool IsRedirecting() {
			return Vector3.Distance(physicalHand.position, virtualHand.position) > Vector3.kEpsilon;
		}

		public void resetRedirection() {
			setTechnique(BRTechnique.None);
		}
	}
}