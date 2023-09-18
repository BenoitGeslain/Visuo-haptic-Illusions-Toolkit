using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BG.Redirection {

    /// <summary> 
    /// This class records the position of various objects of interest.
    /// </summary>
    /// <param name="physicalTarget">The physical target the hand should reach when the virtual hand reaches the virtual target.</param>
    /// <param name="virtualTarget">The virtual target the user is reaching for.</param>
    /// <param name="origin">The point of origin where no redirection is applied.</param>
    /// <param name="physicalHand">The Transform representing the user's physical hand.</param>
    /// <param name="virtualHand">The Transform representing the user's virtual hand, i.e. the user's avatar.</param>
    public record BodyRedirectionScene()
    {
        public Transform physicalTarget;
        public Transform virtualTarget;
        public Transform origin;
        public Transform physicalHand;
        public Transform virtualHand;

        public BodyRedirectionScene(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand):
			this() {

            this.physicalTarget = physicalTarget;
            this.virtualTarget = virtualTarget;
            this.origin = origin;
            this.physicalHand = physicalHand;
            this.virtualHand = virtualHand;
        }
		/// <summary>
		/// Distance between the user's real and virtual hands.
		/// </summary>
		/// <returns></returns>
		public float GetHandRedirectionDistance() => Vector3.Distance(physicalHand.position, virtualHand.position);
		/// <summary>
		/// Distance between the user's real hand and the physical target.
		/// </summary>
		/// <returns></returns>
		public float GetPhysicalHandTargetDistance() => Vector3.Distance(physicalHand.position, physicalTarget.position);
		/// <summary>
		/// Distance between the user's real hand and the origin.
		/// </summary>
		/// <returns></returns>
		public float GetPhysicalHandOriginDistance() => Vector3.Distance(physicalHand.position, origin.position);

        /// <summary>
        /// Reposition the virtual hand at the position given by <c>physicalHand.position + redirection</c>.
        /// </summary>
        /// <param name="redirection"></param>
        public void SetRedirection(Vector3 redirection)
		{
			virtualHand.position = physicalHand.position + redirection;
		}
    }

    /// <summary>
    ///  This class is the most conceptual class of  body redirection defining the important
    ///  functions to call: Redirect().
    /// </summary>
    [Serializable]
	public class BodyRedirectionTechnique {

		[Tooltip("If a2 = 0, then the redirection is linear and equivalent to Han et al., 2018. If a2 is -1/D²<a2<1/D², the redirectionfunction doesn't redirect in the opposite direction (-1/D^2) or over redirects (1/D^2).")]
		[Range(-1f, 1f)]
        public float redirectionLateness;
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
		public virtual void Redirect(BodyRedirectionScene scene) {
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

		public override void Redirect(BodyRedirectionScene scene) {
			var d = scene.physicalTarget.position - scene.origin.position;
			var warpingRatio = Mathf.Clamp(
				Vector3.Dot(d, scene.physicalHand.position - scene.origin.position) / d.sqrMagnitude,
				0f,
				1f);
			scene.SetRedirection(warpingRatio * (scene.virtualTarget.position - scene.physicalTarget.position));
		}
	}

    /// <summary>
    ///
    /// </summary>
    public class Azmandian2016Hybrid: BodyRedirectionTechnique {

		public Azmandian2016Hybrid(BodyRedirection script): base(script) {}

		public override void Redirect(BodyRedirectionScene scene) {
			Debug.Log("Method not implemented yet.");
		}
	}

    /// <summary>
    /// Class for instantly redirecting the user's virtual hand.
    /// </summary>
    public class Han2018Instant: BodyRedirectionTechnique {

		public Han2018Instant(BodyRedirection script): base(script) {}

        public override void Redirect(BodyRedirectionScene scene) {
			// If the hand is inside the redirection boundary, instantly applies the redirection
			if (scene.GetPhysicalHandOriginDistance() > Toolkit.Instance.parameters.NoRedirectionBuffer) {
				scene.SetRedirection(scene.virtualTarget.position - scene.physicalTarget.position);
			} else {
				scene.SetRedirection(Vector3.zero);
			}
		}
    }

    /// <summary>
    /// Class for redirecting the user's virtual hand by an amount that decreases linearly with the target - hand distance.
    /// </summary>
    public class Han2018Continous: BodyRedirectionTechnique {

		public Han2018Continous(BodyRedirection script): base(script) {}

		public override void Redirect(BodyRedirectionScene scene) {
			float D = scene.GetPhysicalHandTargetDistance();
			float B = Vector3.Magnitude(scene.physicalTarget.position - scene.origin.position) + Toolkit.Instance.parameters.NoRedirectionBuffer;
			scene.SetRedirection(Math.Max(1 - D / B, 0f) * (scene.virtualTarget.position- scene.physicalTarget.position));
		}
	}

	public class Cheng2017Sparse: BodyRedirectionTechnique {

		public Cheng2017Sparse(BodyRedirection script): base(script) {}

		public override void Redirect(BodyRedirectionScene scene) {
			float Ds = scene.GetPhysicalHandOriginDistance();
			float Dp = scene.GetPhysicalHandTargetDistance();
			float alpha = Ds / (Ds + Dp);
			scene.SetRedirection(alpha * (scene.virtualTarget.position - scene.physicalTarget.position));
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

		public override void Redirect(BodyRedirectionScene scene) { 
			// The redirection is a degree-2 polynomial function of the distance,
			// f(d) = a_0 + a_1 * d + a_2 * d^2,
			// with limit conditions f(0) = 1 (hence a_0 = 1) and f(D) = 0, where D is the origin - real target distance
			// (hence a_1 * D = 1 - a_2 * D^2).
			// The input parameter redirectionLateness is - a2 * D^2
			float D = Vector3.Distance(scene.physicalTarget.position, scene.origin.position);
			float a2 = this.redirectionLateness / (D * D);
            float[] coeffsByIncreasingPower = { 1f, -1f / D - a2 * D, a2 };
			float d = scene.GetPhysicalHandTargetDistance();
			float ratio = (float) coeffsByIncreasingPower.Select((a, i) => a * Math.Pow(d, i)).Sum();
			scene.SetRedirection(ratio * (scene.virtualTarget.position - scene.physicalTarget.position));
		}
	}




	// Reset the redirection over a short period of time
	public class ResetBodyRedirection: BodyRedirectionTechnique {

		public ResetBodyRedirection(BodyRedirection script): base(script) { }

		public override void Redirect(BodyRedirectionScene scene) { 
			if (this.rootScript.IsRedirecting()) {
				scene.virtualHand.position += Vector3.ClampMagnitude((scene.physicalHand.position - scene.virtualHand.position) * Time.deltaTime, maxLength: 0.0025f);
			}
		}
	}
}