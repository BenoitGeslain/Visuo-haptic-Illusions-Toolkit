using System;
using System.Linq;

using UnityEngine;

namespace VHToolkit.Redirection {

	/// <summary>
	///  This class is the most conceptual class of  world redirection defining the important function to call: Redirect().
	///  Information about the user such as the user's position or the targets are encapsulated inside Scene.
	/// </summary>
	public class WorldRedirectionTechnique : RedirectionTechnique {
		public void CopyHeadAndHandTransform(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();

			scene.CopyHandRotations();
		}
	}

	/// <summary>
	/// This class implements the rotation over time technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by a fixed amount
	/// in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

			scene.RotateVirtualHeadY(GetRedirection(scene));
		}

		public static float GetRedirection(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();
			angleToTarget = (angleToTarget > 180f)? angleToTarget - 360 : angleToTarget;

			return Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalError
				? - Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime
				: 0f;
		}

		public static float GetRedirectionReset(Scene scene) {
			float angleToTarget = scene.GetHeadToHeadRotation().eulerAngles.y;
			angleToTarget = (angleToTarget > 180f)? angleToTarget - 360 : angleToTarget;

			Debug.Log(angleToTarget);
			return Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalError
				? - Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime
				: 0f;
		}
	}

	/// <summary>
	/// This class implements the rotationnal technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by an amount proportional
	/// to their angular speed in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

			scene.RotateVirtualHeadY(GetRedirection(scene));
		}

		public static float GetRedirection(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();
			float instantRotation = scene.GetHeadInstantRotationY();

			if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalError && Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.RotationThreshold) {
				return instantRotation * ((Mathf.Sign(scene.GetHeadAngleToTarget()) == Mathf.Sign(instantRotation))
					? Toolkit.Instance.parameters.GainsRotational.opposite - 1
					: Toolkit.Instance.parameters.GainsRotational.same - 1);
			}
			return 0f;
		}

		public static float GetRedirectionReset(Scene scene) {
			float angleToTarget = scene.GetHeadToHeadRotation().eulerAngles.y;
			float instantRotation = scene.GetHeadInstantRotationY();

			if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.RotationThreshold && Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalError) {
				return instantRotation * ((Mathf.Sign(scene.GetHeadAngleToTarget()) == Mathf.Sign(instantRotation))
					? Toolkit.Instance.parameters.GainsRotational.opposite - 1
					: Toolkit.Instance.parameters.GainsRotational.same - 1);
			}
			return 0f;
		}
	}

	/// <summary>
	/// This class implements the curvature technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by an amount proportional
	/// to their linear speed in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001Curvature: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

			scene.RotateVirtualHeadY(GetRedirection(scene));
		}

		public static float GetRedirection(Scene scene) {
			float instantTranslation = scene.GetHeadInstantTranslationForward().magnitude;

			return instantTranslation > Toolkit.Instance.parameters.WalkingThreshold * Time.deltaTime
				? - Mathf.Sign(Vector3.Cross(scene.physicalHead.forward, scene.forwardTarget).y) * instantTranslation * Toolkit.Instance.CurvatureRadiusToRotationRate()
				: 0f;
		}
	}


	/// <summary>
	/// This class implements the complete Redirected Walking technique from Razzaque et al., 2001. This technique applies the maximum rotation obtained using:
	/// - the over time rotation technique
	/// - the rotationnal technqiue
	/// - the curvature technique
	/// to the user's head.
	/// </summary>
	public class Razzaque2001Hybrid: WorldRedirectionTechnique {

		readonly Func<float, float, float, float> aggregate;

        /// <summary>
        /// By default, the aggregation function is the maximum by absolute value.
        /// </summary>
        public Razzaque2001Hybrid() : base() => aggregate = (a, b, c) => (new float[] { a, b, c }).OrderByDescending(Mathf.Abs).First();

        /// <summary>
        /// Constructor taking a parameter, an aggregation function (float, float, float) -> float.
        /// </summary>
        /// <param name="aggregate"></param>
        public Razzaque2001Hybrid(Func<float, float, float, float> aggregate) : base() => this.aggregate = aggregate;

        /// <summary>
        /// Static factory method for using sum-aggregation.
        /// </summary>
        /// <param name="aggregate"></param>
        static Razzaque2001Hybrid Sum() => new((a, b, c) => a + b + c);

		/// <summary>
		/// Static factory method for using weighted-sum-aggregation.
		/// </summary>
		static Razzaque2001Hybrid Weighted(float x, float y, float z) => new((a, b, c) => a * x + b * y + c * z);

		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

			float angle = aggregate(
				Razzaque2001OverTimeRotation.GetRedirection(scene),
				Razzaque2001Rotational.GetRedirection(scene),
				Razzaque2001Curvature.GetRedirection(scene)
			);

			if (scene.applyDampening) {
				angle = ApplyDampening(scene, angle);
			}
			if (scene.applySmoothing) {
				angle = ApplySmoothing(scene, angle);
			}

			scene.previousRedirection = angle;
			scene.RotateVirtualHeadY(angle);
		}

		private float ApplyDampening(Scene scene, float angle) {
			float dampenedAngle = angle * Mathf.Sin(Mathf.Min(scene.GetHeadAngleToTarget() / Toolkit.Instance.parameters.DampeningRange, 1f) * Mathf.PI/2);
			float dampenedAngleDistance = dampenedAngle * Mathf.Min(scene.GetHeadToTargetDistance() / Toolkit.Instance.parameters.DampeningDistanceThreshold, 1f);
			return (scene.GetHeadToTargetDistance() < Toolkit.Instance.parameters.DampeningDistanceThreshold)? dampenedAngleDistance : dampenedAngle;
		}

		public float ApplySmoothing(Scene scene, float angle) => (1 - Toolkit.Instance.parameters.SmoothingFactor) * scene.previousRedirection + Toolkit.Instance.parameters.SmoothingFactor * angle;
	}

	/// <summary>
	/// This class implements the translationnal technique from Steinicke et al., 2008. This technique scales the user's displacement in order to virtually increase the space
	/// the user can explore freely.
	/// </summary>
	public class Steinicke2008Translational: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

            scene.virtualHead.Translate(Vector3.Scale(scene.GetHeadInstantTranslation(), Toolkit.Instance.parameters.GainsTranslational - Vector3.one), relativeTo: Space.World);
		}
	}

	/// <summary>
	/// This class implements the world warping technique from Azmandian et al., 2016. This technique applies a gain to the user's head rotation in order to co-localize a physical object
	/// and its virtual counterpart.
	/// </summary>
	public class Azmandian2016World: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);

			scene.virtualHead.RotateAround(scene.origin.position, Vector3.up, GetRedirection(scene));
		}

		public static float GetRedirection(Scene scene) {
			float angleBetweenTargets = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalTarget.position - scene.origin.position, Vector3.up), scene.virtualTarget.position - scene.origin.position, Vector3.up);
			float angleBetweenHeads = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.virtualHead.forward, Vector3.up);

			if (Mathf.Abs(angleBetweenTargets - angleBetweenHeads) > Toolkit.Instance.parameters.RotationalError) {
				float angle = angleBetweenTargets - angleBetweenHeads;
				float instantRotation = scene.GetHeadInstantRotationY();

				if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.RotationThreshold && Mathf.Abs(angle) > Toolkit.Instance.parameters.RotationalError) {
					var gain = (Mathf.Sign(angle) == Mathf.Sign(instantRotation)) ? Toolkit.Instance.parameters.GainsRotational.same : Toolkit.Instance.parameters.GainsRotational.opposite;
					var bound = Mathf.Abs(gain * instantRotation);
					return Mathf.Clamp(angle, -bound, bound);
				}
			}
			return 0f;
		}
	}

	/// <summary>
	/// This class does not implement a redirection technique but reset the rotation between the user's physical and virtual head by using the over time rotation
	/// and rotationnal technique from Razzaque et al., 2001.
	/// </summary>
	public class ResetWorldRedirection: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {

		}
	}

	public class NoWorldRedirection: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			CopyHeadAndHandTransform(scene);
		}
	}
}