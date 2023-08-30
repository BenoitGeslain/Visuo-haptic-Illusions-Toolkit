using UnityEngine;

namespace BG.Redirection {

	public enum BRTechnique {
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
	public class BodyRedirection : MonoBehaviour {

		[SerializeField] private BRTechnique technique;
		[SerializeField] private BodyRedirectionTechnique techniqueClass;

		[Header("User Parameters")]
		[SerializeField] private Transform realHand;
		[SerializeField] private Transform virtualHand;

		[Header("Technique Parameters")]
		public Transform origin;
		public Transform realTarget;
		public Transform virtualTarget;

		private bool reset;

		private void init() {
			switch (technique) {
				case BRTechnique.Azmandian2016Body:
					techniqueClass = new Azmandian2016Body();
					break;
				case BRTechnique.Azmandian2016World:
					techniqueClass = new Azmandian2016World();
					break;
				case BRTechnique.Azmandian2016Hybrid:
					techniqueClass = new Azmandian2016Hybrid();
					break;
				case BRTechnique.Han2018Instant:
					techniqueClass = new Han2018Instant();
					break;
				case BRTechnique.Han2018Continous:
					techniqueClass = new Han2018Continous();
					break;
				case BRTechnique.Cheng2017Sparse:
					techniqueClass = new Cheng2017Sparse();
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					break;
			}
		}

		private void Start() {
			init();
		}

		private void Update() {
			if (!reset) {
				techniqueClass.Redirect(realTarget, virtualTarget, origin, realHand, virtualHand);
			} else {
				// Reset virtualHand to realHand progressively
			}
		}

		public void setTechnique(BRTechnique t) {
			technique = t;
			init();
		}
		public BRTechnique getTechnique(BRTechnique t) {
			return technique;
		}

		public bool IsRedirecting() {
			return Vector3.Distance(realHand.position, virtualHand.position) < Vector3.kEpsilon;
		}

		public void resetRedirection() {
			reset = true;
		}

		public void restartRedirection() {
			if (reset) {
				Debug.LogWarning("Redirection should be reset before restarted.");
				return;
			} else if (IsRedirecting()) {
				Debug.LogWarning("Redirection is not reset yet");
				return;
			}
			reset = false;
		}
	}
}