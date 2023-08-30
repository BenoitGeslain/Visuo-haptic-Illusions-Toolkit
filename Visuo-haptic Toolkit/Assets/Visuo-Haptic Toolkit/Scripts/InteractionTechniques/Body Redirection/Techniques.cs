using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  body redirection defining the important
    ///  functions to call: Redirect()
	/// </summary>
	public class BodyRedirectionTechnique {

		public virtual void InitRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Azmandian2016Body: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016Hybrid: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Han2018Instant: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
            virtualHand.position = physicalHand.position + (physicalTarget.position - virtualTarget.position);
        }
	}

	public class Han2018Continous: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			float D = Vector3.Distance(physicalTarget.position, physicalHand.position);
			float B = Vector3.Magnitude(physicalTarget.position - physicalHand.position) + 0.1f;

			if (D>=B) {		// 1:1 mapping
				virtualHand.position = physicalHand.position;
			} else {		// Inside redirection boundary
				virtualHand.position = physicalHand.position + (physicalTarget.position - virtualTarget.position)*(1-D/B);
			}
        }
	}

	public class Cheng2017Sparse: BodyRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}
}