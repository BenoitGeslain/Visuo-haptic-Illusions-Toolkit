using UnityEngine;

namespace BG.VHIllusion {

	public class BodyRedirection : MonoBehaviour {
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

		static readonly Vector3[] vectorAxes = new Vector3[] {
			Vector3.right,
			Vector3.up,
			Vector3.forward
		};

		public Technique techniqueEnum;
		private BodyRedirectionTechnique technique;

		public GameObject realHand;
		public GameObject virtualHand;
		public GameObject invariant;
		public Axis direction;

		public bool redirecting;

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
	}
}