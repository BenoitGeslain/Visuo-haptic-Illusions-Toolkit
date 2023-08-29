using UnityEngine;

namespace BG.Redirection {

	public enum Technique {
		Han2018Instant,
		Han2018Continous,
		Azmandian2016Body,
		Azmandian2016World,
		Azmandian2016Hybrid,
		Cheng2017Sparse
	}

	public enum Axis {
		X,
		Y,
		Z
	}

	public class BodyRedirection : MonoBehaviour {

		// static readonly Vector3[] vectorAxes = new Vector3[] {
		// 	Vector3.right,
		// 	Vector3.up,
		// 	Vector3.forward
		// };

		[SerializeField] private Technique technique;
		[SerializeField] private BodyRedirectionTechnique techniqueClass;

		[Header("Technique Parameters")]
		[SerializeField] private Transform realHand;
		[SerializeField] private Transform virtualHand;
		[SerializeField] private Transform invariant;
		[SerializeField] private Transform realTarget;
		[SerializeField] private Transform virtualTarget;
		// [SerializeField] private Axis direction;

		[SerializeField] private bool redirecting;

		void Start() {
			redirecting = true;
			switch (technique) {
				case Technique.Azmandian2016Body:
					techniqueClass = new Azmandian2016Body();
					break;
				case Technique.Azmandian2016World:
					techniqueClass = new Azmandian2016World();
					break;
				case Technique.Azmandian2016Hybrid:
					techniqueClass = new Azmandian2016Hybrid();
					break;
				case Technique.Han2018Instant:
					techniqueClass = new Han2018Instant();
					break;
				case Technique.Han2018Continous:
					techniqueClass = new Han2018Continous();
					break;
				case Technique.Cheng2017Sparse:
					techniqueClass = new Cheng2017Sparse();
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					redirecting = false;
					break;
			}
		}

		void Update() {
			techniqueClass.Redirect(realTarget, virtualTarget, invariant, realHand, virtualHand);
		}

		public bool IsRedirecting() {
			return redirecting;
		}
	}
}