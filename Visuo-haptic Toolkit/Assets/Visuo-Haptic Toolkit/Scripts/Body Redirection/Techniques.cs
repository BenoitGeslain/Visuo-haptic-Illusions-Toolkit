using UnityEngine;


namespace BG.Redirection {
	public class BodyRedirectionTechnique {

		public virtual void Redirect(Transform realTarget, Transform virtualTarget, Transform origin, Transform realHand, Transform virtualHand) {
			Debug.LogError("Virtual Method.");
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