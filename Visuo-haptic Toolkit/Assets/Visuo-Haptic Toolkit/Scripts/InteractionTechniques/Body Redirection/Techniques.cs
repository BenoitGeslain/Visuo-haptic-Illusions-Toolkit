using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  body redirection defining the important
	///  functions to call: Redirect().
	/// </summary>
	public class BodyRedirectionTechnique {

		protected BodyRedirection rootScript;

        public BodyRedirectionTechnique(BodyRedirection script) => this.rootScript = script;

        public virtual void InitRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		/// <summary>
		/// This virtual function applies the redirection to the virtualHand Transform according to the other parameters and the equations
		/// defined in the corresponding techniques.
		/// </summary>
		/// <param name="physicalTarget">The physical target the hand should reach when the virtual hand reaches the virtual target.</param>
		/// <param name="virtualTarget">The virtual target the user is reaching for.</param>
		/// <param name="origin">The point of origin where no redirection is applied.</param>
		/// <param name="physicalHand">The Transform representing the user's physical hand.</param>
		/// <param name="virtualHand">The Transform representing the user's virtual hand, i.e. the user's avatar.</param>
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
			var d = physicalTarget.position - origin.position;
			var warpingRatio = Mathf.Clamp(
				Vector3.Dot(d, physicalHand.position - origin.position) / d.sqrMagnitude,
				0f,
				1f);
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

        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			// If the hand is inside the redirection boundary, instantly applies the redirection
			if (Vector3.Magnitude(physicalHand.position - origin.position) > Toolkit.Instance.parameters.NoRedirectionBuffer) {
				virtualHand.position = physicalHand.position + virtualTarget.position - physicalTarget.position;
			} else {
				virtualHand.position = physicalHand.position;
			}
		}
    }

	public class Han2018Continous: BodyRedirectionTechnique {

		public Han2018Continous(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			float D = Vector3.Magnitude(physicalTarget.position - physicalHand.position);
			float B = Vector3.Magnitude(physicalTarget.position - origin.position) + Toolkit.Instance.parameters.NoRedirectionBuffer;
			virtualHand.position = physicalHand.position + Math.Max(1 - D / B, 0f) * (virtualTarget.position - physicalTarget.position);
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

	public class Geslain2022Polynom: BodyRedirectionTechnique {

		public Geslain2022Polynom(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			Debug.Log("Method not implemented yet.");
			float[] coeffsByIncreasingPower = { 0f, 1f, 0f }; // TODO compute coeffs - current default is instant redirection
			float d = Vector3.Distance(physicalHand.position, physicalTarget.position);
			float ratio = (float) coeffsByIncreasingPower.Select((a, i) => a * Math.Pow(d, i)).Sum();
			virtualHand.position = physicalHand.position + ratio * (virtualTarget.position - physicalTarget.position);
		}
	}




	// Reset the redirection over a short period of time
	public class ResetRedirection: BodyRedirectionTechnique {

		public ResetRedirection(BodyRedirection script): base(script) { }

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			if (Vector3.Distance(physicalHand.position, virtualHand.position) > Vector3.kEpsilon) {
				virtualHand.position += Vector3.ClampMagnitude((physicalHand.position - virtualHand.position) * Time.deltaTime, maxLength: 0.0025f);
			}
		}
	}
}