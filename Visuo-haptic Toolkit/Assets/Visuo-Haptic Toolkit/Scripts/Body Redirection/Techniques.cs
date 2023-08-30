using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  body redirection defining the important
    ///  functions to call: Redirect()
	/// </summary>
	public class BodyRedirectionTechnique {

		public virtual void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Azmandian2016Body: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016Hybrid: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Han2018Instant: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Han2018Continous: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Cheng2017Sparse: BodyRedirectionTechnique {
        public override void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
            Debug.Log("Method not implemented yet.");
        }
	}
}