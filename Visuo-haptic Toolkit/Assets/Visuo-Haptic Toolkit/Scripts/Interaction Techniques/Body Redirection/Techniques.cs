using System;
using System.Linq;

using UnityEngine;

namespace BG.Redirection {
    /// <summary>
    ///  This class is the most conceptual class of body interaction techniques defining the important function to call: Redirect().
	///  Information about the user such as the user's position or the targets are encapsulated inside Scene.
    /// </summary>
    [Serializable]
	public class BodyRedirectionTechnique : RedirectionTechnique {

		[Tooltip("If a2 = 0, then the redirection is linear and equivalent to Han et al., 2018. If a2 is -1/D²<a2<1/D², the redirectionfunction doesn't redirect in the opposite direction (-1/D^2) or over redirects (1/D^2).")]
		[Range(-1f, 1f)]
        public float redirectionLateness;
		public Vector2 controlPoint;
	}

	/// <summary>
	/// This class implements the Body Warping technique from Azmandian et al., 2016. This technique redirects the user's virtual hand by an
	/// amount proportional to the sine of the real target - origin - real hand angle and to the real hand - origin distance, clamped between 0 and 1.
	/// </summary>
	public class Azmandian2016Body: BodyRedirectionTechnique {

        public override void Redirect(Scene scene) => scene.Redirection = GetRedirection(scene);

        public static Vector3 GetRedirection(Scene scene) {
			var d = scene.physicalTarget.position - scene.origin.position;
			var warpingRatio = Mathf.Clamp(
				Vector3.Dot(d, scene.physicalHand.position - scene.origin.position) / d.sqrMagnitude,
				0f,
				1f);
			return warpingRatio * (scene.virtualTarget.position - scene.physicalTarget.position);
		}
	}

    /// <summary>
    /// This class implements the Hybrid Warping technique from Azmandian et al., 2016. This technique combines Body Warping and World Warping from the same author.
	/// Both technique apply at the same time but World Warping is prioritised when the user's hand is far from the target, conversly, Body Warping is prioritised
	/// when the user's hand is closer to the target. This interpolation follows a simple linear function.
    /// </summary>
    public class Azmandian2016Hybrid: BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();

			float handTargetDistance = Vector3.Dot(scene.origin.position - scene.physicalTarget.position, scene.physicalHand.position - scene.physicalTarget.position);
			handTargetDistance = Mathf.Clamp(handTargetDistance, 0f, 1f);

			Vector3 BRRedirection = (1 - handTargetDistance) * Azmandian2016Body.GetRedirection(scene);
			float WRRedirection = handTargetDistance * Azmandian2016World.GetRedirection(scene);

			scene.Redirection = BRRedirection;
			scene.virtualHead.RotateAround(scene.origin.position, Vector3.up, WRRedirection);
		}
	}

    /// <summary>
	/// This class implements the Translation Shift technique from Han et al., 2018 renamed Instant in this toolkit as Instant. This technique instantly redirects the user's hand
	/// to remap the the virtual target to the physical one.
    /// </summary>
    public class Han2018Instant: BodyRedirectionTechnique {

        public override void Redirect(Scene scene) {
			// If the hand is inside the redirection boundary, instantly applies the redirection
			scene.Redirection = scene.GetPhysicalHandOriginDistance() > Toolkit.Instance.parameters.NoRedirectionBuffer
                ? scene.virtualTarget.position - scene.physicalTarget.position
                : Vector3.zero;
        }
    }

    /// <summary>
	/// This class implements the Interpolated Reach technique from Han et al., 2018 renamed Continous in this toolkit as Instant. This technique progressively redirects the user
	/// as they get closer to the physical target in a linear way.
    /// </summary>
    public class Han2018Continous: BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			float D = scene.GetPhysicalHandTargetDistance();
			float B = Vector3.Magnitude(scene.physicalTarget.position - scene.origin.position) + Toolkit.Instance.parameters.NoRedirectionBuffer;
			scene.Redirection = Math.Max(1 - D / B, 0f) * (scene.virtualTarget.position- scene.physicalTarget.position);
		}
	}

	/// <summary>
	/// This class implements the Sparse Haptic technique from Cheng et al., 2017. This technique progressively redirects the user similarily to Han et al., 2018.
	/// </summary>
	public class Cheng2017Sparse: BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			float Ds = scene.GetPhysicalHandOriginDistance();
			float Dp = scene.GetPhysicalHandTargetDistance();
			float alpha = Ds / (Ds + Dp);
			scene.Redirection = alpha * (scene.virtualTarget.position - scene.physicalTarget.position);
		}
	}

    /// <summary>
	/// This class implements the Polynom technique from Geslain et al., 2022. This technique progressively redirects the user using a second degree order polynom
	/// of the hand-target distance.
    /// </summary>
    [Serializable]
	public class Geslain2022Polynom: BodyRedirectionTechnique {

		/// <summary>
		/// Constructor of Geslain2022Polynom
		/// </summary>
		/// <param name="redirectionLateness">This parameter defines the shape of the 2nd order polynom. There are 3 important values:
		/// a2 = 0 simplifies the polynom function to a linear one identical to Han et al. 2018.	// TODO A vérifier
		/// a2 = 1/D^2 makes the redirection be applied more at the **start** of the redirection, i.e. when the user is **far** from the target.
		/// a2 = -1/D^2 makes the redirection be applied more at the **end** of the redirection, i.e. when the user is **close** from the target</param>
		/// <param name="controlPoint">This variable offers another way to parametrise the polynom. The control point represent the second control point of
		/// a Quadratic Bézier curve (three point Bézier curve).</param>
		public Geslain2022Polynom(float redirectionLateness, Vector2 controlPoint): base() {
			this.redirectionLateness = redirectionLateness;	// TODO rename
			this.controlPoint = controlPoint;
		}

		/// The redirection is a degree-2 polynomial function of the distance,
		/// f(d) = a_0 + a_1 * d + a_2 * d^2,
		/// with limit conditions f(0) = 1 (hence a_0 = 1) and f(D) = 0, where D is the origin - real target distance
		/// (hence a_1 * D = 1 - a_2 * D^2).
		/// The input parameter redirectionLateness is a2 * D^2
		public override void Redirect(Scene scene) {
			float D = Vector3.Distance(scene.physicalTarget.position, scene.origin.position);
			float a2 = this.redirectionLateness / (D * D);
            float[] coeffsByIncreasingPower = { 1f, -1f / D - a2 * D, a2 };	// {a0, a1, a2}
			float d = scene.GetPhysicalHandTargetDistance();
			float ratio = (float) coeffsByIncreasingPower.Select((a, i) => a * Math.Pow(d, i)).Sum();
			scene.Redirection = ratio * (scene.virtualTarget.position - scene.physicalTarget.position);
		}
	}

    /// <summary>
    /// This class implements the GoGo technique from Poupyrev et al., 1996. It is not a visuo-haptic illusion but an interaction techique allowing to extend the
	/// reach of the user by breaking the 1:1 mapping when the user's hand is 2/3 from their chest (GoGoActivationDistance in toolkit parameters). The redirection
	/// follows a second order polynom of the chest to hand distance.
    /// </summary>
    [Serializable]
	public class Poupyrev1996GoGo: BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			// Offset the head position by 0.2m to approximate the chest position then compute the chest to hand vector
			Vector3 chestToHand = scene.physicalHand.position + new Vector3(0f, 0.2f, 0f) - scene.physicalHead.position;
			scene.Redirection = chestToHand.magnitude > Toolkit.Instance.parameters.GoGoActivationDistance
                ? Toolkit.Instance.parameters.GoGoCoefficient * Mathf.Pow(chestToHand.magnitude - Toolkit.Instance.parameters.GoGoActivationDistance, 2) * chestToHand.normalized
                : Vector3.zero;
		}
	}

    /// <summary>
    /// This class does not implement a redirection technique but resets the redirection currently applied to the user's hand by slowly reducing the virtual to
    /// physical distance to 0.
    /// </summary>
    public class ResetBodyRedirection : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			Vector3 instantTranslation = scene.GetHandInstantTranslation();
			scene.virtualHand.position += instantTranslation + instantTranslation.magnitude * Toolkit.Instance.parameters.ResetRedirectionCoeff * (scene.physicalHand.position - scene.virtualHand.position).normalized;
		}
	}
}