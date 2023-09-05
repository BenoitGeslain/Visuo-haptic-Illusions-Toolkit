using Unity.VisualScripting;
using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  body redirection defining the important
	///  functions to call: Redirect()
	/// </summary>
	public class BodyRedirectionTechnique {

		protected BodyRedirection rootScript;

        public BodyRedirectionTechnique(BodyRedirection script) => this.rootScript = script;

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

		public Azmandian2016Body(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			var d = virtualTarget.position - origin.position;
			var warpingRatio = Vector3.Dot(d, physicalHand.position - origin.position) / d.sqrMagnitude;
			virtualHand.position = physicalHand.position + warpingRatio * (virtualTarget.position - physicalTarget.position);
		}
	}

	public class Azmandian2016World: BodyRedirectionTechnique {

		public Azmandian2016World(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			Debug.Log("Method not implemented yet.");
		}
	}

	public class Azmandian2016Hybrid: BodyRedirectionTechnique {

		public Azmandian2016Hybrid(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			Debug.Log("Method not implemented yet.");
		}
	}

	public class Han2018Instant: BodyRedirectionTechnique {

		public Han2018Instant(BodyRedirection script): base(script) {}

        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) => 
			virtualHand.position = physicalHand.position + virtualTarget.position - physicalTarget.position;
    }

	public class Han2018Continous: BodyRedirectionTechnique {

		public Han2018Continous(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			float D = Vector3.Magnitude(physicalTarget.position - physicalHand.position);
			float B = Vector3.Magnitude(physicalTarget.position - physicalHand.position) + Toolkit.Instance.parameters.NoRedirectionBuffer;

			if (D >= B) {		// 1:1 mapping
				virtualHand.position = physicalHand.position;
			} else {		// Inside redirection boundary
				virtualHand.position = physicalHand.position - (physicalTarget.position - virtualTarget.position) * (1 - D / B);
			}
		}
	}

	public class Cheng2017Sparse: BodyRedirectionTechnique {

		public Cheng2017Sparse(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			float Ds = Vector3.Distance(physicalHand.position, origin.position);
			float Dp = Vector3.Distance(physicalHand.position, physicalTarget.position);
			float alpha = Ds / (Ds + Dp);
			virtualHand.position = physicalHand.position + alpha * (virtualTarget.position - physicalTarget.position);
		}
	}




	// Reset the redirection over a short period of time
	public class ResetRedirection: BodyRedirectionTechnique {

		public ResetRedirection(BodyRedirection script): base(script) { }

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			if (Vector3.Distance(physicalHand.position, virtualHand.position) > Vector3.kEpsilon) {
				virtualHand.position = virtualHand.position - Vector3.ClampMagnitude((virtualHand.position - physicalHand.position) * Time.deltaTime, maxLength: 0.0025f);
			}
		}
	}
}