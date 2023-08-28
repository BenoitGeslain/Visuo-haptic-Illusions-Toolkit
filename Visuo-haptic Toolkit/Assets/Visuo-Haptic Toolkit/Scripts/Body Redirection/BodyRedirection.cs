using UnityEngine;

namespace BG.VHIllusion {

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

		static readonly Vector3[] vectorAxes = new Vector3[] {
			Vector3.right,
			Vector3.up,
			Vector3.forward
		};

		[SerializeField] private Technique techniqueEnum;
		[SerializeField] private BodyRedirectionTechnique technique;

		[SerializeField] private GameObject realHand;
		[SerializeField] private GameObject virtualHand;
		[SerializeField] private GameObject invariant;
		[SerializeField] private Axis direction;

		[SerializeField] private bool redirecting;

		void Start() {
			redirecting = true;
			switch (techniqueEnum) {
				case Technique.Azmandian2016Body:
					technique = new Azmandian2016Body();
					break;
				case Technique.Azmandian2016World:
					technique = new Azmandian2016World();
					break;
				case Technique.Azmandian2016Hybrid:
					technique = new Azmandian2016Hybrid();
					break;
				case Technique.Han2018Instant:
					technique = new Han2018Instant();
					break;
				case Technique.Han2018Continous:
					technique = new Han2018Continous();
					break;
				case Technique.Cheng2017Sparse:
					technique = new Cheng2017Sparse();
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					redirecting = false;
					break;
			}
		}

		void Update() {
			technique.Redirect(realHand, virtualHand, invariant, vectorAxes[(int)direction]);
		}

		public bool IsRedirecting() {
			return redirecting;
		}
	}
}