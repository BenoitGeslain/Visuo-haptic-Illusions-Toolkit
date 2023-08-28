using UnityEngine;


namespace BG.VHIllusion {
	public class BodyRedirectionTechnique {

		public virtual void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
			Debug.LogError("Virtual Method.");
		}
	}

	public class Azmandian2016Body: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016Hybrid: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Han2018Instant: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Han2018Continous: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Cheng2017Sparse: BodyRedirectionTechnique {
        public override void Redirect(GameObject realHand, GameObject virtualHand, GameObject invariant, Vector3 direction) {
            Debug.Log("Method not implemented yet.");
        }
	}
}