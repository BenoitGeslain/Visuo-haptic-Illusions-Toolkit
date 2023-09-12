using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  body redirection defining the important
	///  functions to call: Redirect().
	/// </summary>
	[Serializable]
	public class BodyRedirectionTechnique {

		[Tooltip("The coefficient a2 from the second order polynom: f(d) = a_0 + a_1 * d + a_2 * d^2. If a2 = 0, then the redirection is linear and equivalent to Han et al., 2018. If a2 is -1/D²<a2<1/D², the redirectionfunction doesn't redirect in the opposite direction (-1/D^2) or over redirects (1/D^2).")]
		[Range(-10.0f, 10.0f)]
        public float a2;
		public Vector2 controlPoint;

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

	/// <summary>
	/// Class for redirecting the user's virtual hand by an amount proportional to the sine of the
	/// real target - origin - real hand angle and to the real hand - origin distance, clamped.
	/// </summary>
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

    /// <summary>
    ///
    /// </summary>
    public class Azmandian2016Hybrid: BodyRedirectionTechnique {

		public Azmandian2016Hybrid(BodyRedirection script): base(script) {}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			Debug.Log("Method not implemented yet.");
		}
	}

    /// <summary>
    /// Class for instantly redirecting the user's virtual hand.
    /// </summary>
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

    /// <summary>
    /// Class for redirecting the user's virtual hand by an amount that decreases linearly with the target - hand distance.
    /// </summary>
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


	/// <summary>
	/// Class for redirecting the user's hand by an amount that has a degree-2 polynomial dependency on the hand-target distance.
	/// </summary>
	[Serializable]
	public class Geslain2022Polynom: BodyRedirectionTechnique {


		public Geslain2022Polynom(BodyRedirection script, float redirectionLateness, Vector2 controlPoint): base(script) {
			this.redirectionLateness = redirectionLateness;
			this.controlPoint = controlPoint;
		}

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {

			// The redirection is a degree-2 polynomial function of the distance,
			// f(d) = a_0 + a_1 * d + a_2 * d^2,
			// with limit conditions f(0) = 1 (hence a_0 = 1) and f(D) = 0, where D is the origin - real target distance
			// (hence a_1 * D = 1 - a_2 * D�).
			float D = Vector3.Distance(physicalTarget.position, origin.position);
			float a2 = -this.redirectionLateness / (D * D);
            float[] coeffsByIncreasingPower = { 1f, -1f / D - a2 * D, a2 };
            float d = Vector3.Distance(physicalHand.position, physicalTarget.position);
			float ratio = (float) coeffsByIncreasingPower.Select((a, i) => a * Math.Pow(d, i)).Sum();
			virtualHand.position = physicalHand.position + ratio * (virtualTarget.position - physicalTarget.position);
		}
	}




	// Reset the redirection over a short period of time
	public class ResetBodyRedirection: BodyRedirectionTechnique {

		public ResetBodyRedirection(BodyRedirection script): base(script) { }

		public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand) {
			if (Vector3.Distance(physicalHand.position, virtualHand.position) > Vector3.kEpsilon) {
				virtualHand.position += Vector3.ClampMagnitude((physicalHand.position - virtualHand.position) * Time.deltaTime, maxLength: 0.0025f);
			}
		}
	}
}