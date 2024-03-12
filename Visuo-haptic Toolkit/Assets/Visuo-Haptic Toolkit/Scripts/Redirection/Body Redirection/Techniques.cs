using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Redirection.BodyRedirection {
	/// <summary>
	///  This class is the most conceptual class of body interaction techniques defining the important function to call: Redirect().
	///  Information about the user such as the user's position or the targets are encapsulated inside Scene.
	/// </summary>
	[Serializable]
	public abstract class BodyRedirectionTechnique : RedirectionTechnique {
		/// <summary>
		/// Standard bump function used for mollifying the redirection distribution, as is thouroughly common in various fields of applied mathematics and numerical sciences.
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="bufferDistance"></param>
		/// <returns></returns>
        protected static float BumpFunction(float distance, float bufferDistance) => (distance < bufferDistance) ?
                1f - Mathf.Exp(1f + 1 / (Mathf.Pow(distance / bufferDistance, 2) - 1f)) : 1f;
    }

	/// <summary>
	/// This class implements the Body Warping technique from Azmandian et al., 2016. This technique redirects the user's virtual hand by an
	/// amount proportional to the sine of the real target - origin - real hand angle and to the real hand - origin distance, clamped between 0 and 1.
	/// </summary>
	public class Azmandian2016Body : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) => scene.LimbRedirection = GetRedirection(scene);

		public static List<Vector3> GetRedirection(Scene scene) => scene.limbs.ConvertAll(
			limb => {
				var d = scene.physicalTarget.position - scene.origin.position;
				var warpingRatio = Mathf.Clamp01(Vector3.Dot(d, limb.physicalLimb.position - scene.origin.position) / d.sqrMagnitude);
				return warpingRatio * BumpFunction((limb.physicalLimb.position - scene.origin.position).magnitude, scene.parameters.RedirectionBuffer) * (scene.virtualTarget.position - scene.physicalTarget.position);
			}
		);
	}

	/// <summary>
	/// This class implements the Hybrid Warping technique from Azmandian et al., 2016. This technique combines Body Warping and World Warping from the same author.
	/// Both techniques apply at the same time but World Warping is prioritised when the user's hand is far from the target, conversely, Body Warping is prioritised
	/// when the user's hand is closer to the target. This interpolation follows a simple linear function.
	/// </summary>
	public class Azmandian2016Hybrid : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();

			List<float> handTargetDistance = scene.limbs.ConvertAll(limb =>
				Mathf.Clamp01(Vector3.Dot(scene.origin.position - scene.physicalTarget.position, limb.physicalLimb.position - scene.physicalTarget.position)));

			List<Vector3> BRRedirection = handTargetDistance.Zip(Azmandian2016Body.GetRedirection(scene), (v, d) => (1 - v) * d).ToList();
			float WRRedirection = handTargetDistance.FirstOrDefault() * Azmandian2016World.GetRedirection(scene);
			// todo follow multiple limbs and not just first limb

			scene.LimbRedirection = BRRedirection;
			scene.virtualHead.RotateAround(scene.origin.position, Vector3.up, WRRedirection);
		}
	}

	/// <summary>
	/// This class implements the Translation Shift technique from Han et al., 2018 renamed Instant in this toolkit as Instant. This technique instantly redirects the user's hand
	/// to remap the the virtual target to the physical one.
	/// </summary>
	public class Han2018TranslationalShift : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			// If the hand is inside the redirection boundary, instantly applies the redirection
			scene.LimbRedirection = scene.GetPhysicalHandOriginDistance().ConvertAll(
				d => d > scene.parameters.RedirectionBuffer
				? scene.virtualTarget.position - scene.physicalTarget.position
				: Vector3.zero
			);
		}
	}

	/// <summary>
	/// This class implements the Interpolated Reach technique from Han et al., 2018 renamed Continous in this toolkit as Instant. This technique progressively redirects the user
	/// as they get closer to the physical target in a linear way.
	/// </summary>
	public class Han2018InterpolatedReach : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			List<float> D = scene.GetPhysicalHandTargetDistance();
			float B = Vector3.Magnitude(scene.physicalTarget.position - scene.origin.position);
			var C = scene.GetPhysicalHandOriginDistance().Select(c => BumpFunction(c, scene.parameters.RedirectionBuffer));
			var physicalToVirtual = scene.virtualTarget.position - scene.physicalTarget.position;
			scene.LimbRedirection = D.Zip(C, (d, c) => Math.Max(1 - d / B, 0f) * c * physicalToVirtual).ToList();
		}
	}

	/// <summary>
	/// This class implements the Sparse Haptic technique from Cheng et al., 2017. This technique progressively redirects the user similarily to Han et al., 2018.
	/// </summary>
	public class Cheng2017Sparse : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			List<float> alpha = scene.GetPhysicalHandOriginDistance().Zip(scene.GetPhysicalHandTargetDistance(), (s, p) => s / (s + p) * BumpFunction(s, scene.parameters.RedirectionBuffer)).ToList();
			var physicalToVirtual = scene.virtualTarget.position - scene.physicalTarget.position;
			scene.LimbRedirection = alpha.ConvertAll(a => a * physicalToVirtual);
		}
	}

	/// <summary>
	/// This class implements the Polynom technique from Geslain et al., 2022. This technique progressively redirects the user using a second degree order polynom
	/// of the hand-target distance.
	/// </summary>
	[Serializable]
	public class Geslain2022Polynom : BodyRedirectionTechnique {

		/// The redirection is a degree-2 polynomial function of the distance,
		/// f(d) = a_0 + a_1 * d + a_2 * d^2,
		/// with limit conditions f(0) = 1 (hence a_0 = 1) and f(D) = 0, where D is the origin - real target distance
		/// (hence a_1 * D = 1 - a_2 * D^2).
		/// The input parameter redirectionLateness is a2 * D^2
		public override void Redirect(Scene scene) {
			float D = Vector3.Distance(scene.physicalTarget.position, scene.origin.position);
			float a2 = scene.parameters.RedirectionLateness / (D * D);
			float[] coeffsByIncreasingPower = { 1f, -1f / D - a2 * D, a2 }; // {a0, a1, a2}
			List<float> ratio = scene.GetPhysicalHandTargetDistance().Zip(scene.GetPhysicalHandOriginDistance(), (s, p) => BumpFunction(p, scene.parameters.RedirectionBuffer) * (float)coeffsByIncreasingPower.Select((a, i) => a * Math.Pow(s, i)).Sum()).ToList();
			scene.LimbRedirection = ratio.ConvertAll(r => r * (scene.virtualTarget.position - scene.physicalTarget.position));
		}
	}

	/// <summary>
	/// This class implements the GoGo technique from Poupyrev et al., 1996. It is not a visuo-haptic illusion but an interaction techique allowing to extend the
	/// reach of the user by breaking the 1:1 mapping when the user's hand is 2/3 from their chest (GoGoActivationDistance in toolkit parameters). The redirection
	/// follows a second order polynom of the chest to hand distance.
	/// </summary>
	[Serializable]
	public class Poupyrev1996GoGo : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			// Offset the head position by 0.2m to approximate the chest position then compute the chest to hand vector
			List<Vector3> chestToHand = scene.limbs.ConvertAll(limb => limb.physicalLimb.position + 0.2f * Vector3.up - scene.physicalHead.position);
			scene.LimbRedirection = chestToHand.ConvertAll(d => d.magnitude > scene.parameters.GoGoActivationDistance
				? scene.parameters.GoGoCoefficient * Mathf.Pow(d.magnitude - scene.parameters.GoGoActivationDistance, 2) * d.normalized
				: Vector3.zero);
		}
	}

	/// <summary>
	/// This class does not implement a redirection technique but resets the redirection currently applied to the user's hand by slowly reducing the virtual to
	/// physical distance to 0.
	/// </summary>
	public class ResetBodyRedirection : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			foreach (var (limb, t) in scene.limbs.Zip(scene.GetLimbInstantTranslation())) {
				limb.virtualLimb.ForEach(vlimb => vlimb.Translate(t + t.magnitude * Toolkit.Instance.parameters.ResetRedirectionCoeff * (limb.physicalLimb.position - vlimb.position).normalized, relativeTo: Space.World));
			};
		}
	}

	/// <summary>
	/// This class does not implement a redirection technique but resets the redirection currently applied to the user's hand by slowly reducing the virtual to
	/// physical distance to 0.
	/// </summary>
	public class NoBodyRedirection : BodyRedirectionTechnique {

		public override void Redirect(Scene scene) {
			foreach (var (limb, t) in scene.limbs.Zip(scene.GetLimbInstantTranslation())) {
				limb.virtualLimb.ForEach(vlimb => vlimb.Translate(t, relativeTo: Space.World));
			}
		}
	}
}