using System;
using System.Linq;

using UnityEngine;

namespace VHToolkit.Redirection.WorldRedirection {

	/// <summary>
	///  This class is the most conceptual class of  world redirection defining the important function to call: Redirect().
	///  Information about the user such as the user's position or the targets are encapsulated inside Scene.
	/// </summary>
	public abstract class WorldRedirectionTechnique : RedirectionTechnique {
		public void CopyHeadAndLimbTransform(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();
			scene.CopyLimbTranslationAndRotation();
		}
	}

	/// <summary>
	/// This class implements the rotation over time technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by a fixed amount
	/// in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001OverTimeRotation : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.RotateVirtualHeadY(GetRedirection(scene));
			CopyHeadAndLimbTransform(scene);
		}

		public static float GetRedirection(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();
			angleToTarget = (angleToTarget > 180f) ? angleToTarget - 360f : angleToTarget;

			return Mathf.Abs(angleToTarget) > scene.parameters.RotationalError
				? Mathf.Sign(angleToTarget) * scene.parameters.OverTimeRotation * Time.deltaTime
				: 0f;
		}

		public static float GetRedirectionReset(Scene scene) {
			float angleToTarget = scene.HeadToHeadRedirection.eulerAngles.y;
			angleToTarget = (angleToTarget > 180f) ? angleToTarget - 360f : angleToTarget;

			Debug.Log(angleToTarget);
			return Mathf.Abs(angleToTarget) > scene.parameters.RotationalError
				? -Mathf.Sign(angleToTarget) * scene.parameters.OverTimeRotation * Time.deltaTime
				: 0f;
		}
	}

	/// <summary>
	/// This class implements the rotationnal technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by an amount proportional
	/// to their angular speed in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001Rotational : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.RotateVirtualHeadY(GetRedirection(scene));
			CopyHeadAndLimbTransform(scene);
		}

		public static float GetRedirection(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();
			float instantRotation = scene.GetHeadInstantRotationY();

			if (Mathf.Abs(angleToTarget) > scene.parameters.RotationalError && Mathf.Abs(instantRotation) > scene.parameters.RotationalThreshold) {
				return instantRotation * ((Mathf.Sign(angleToTarget) != Mathf.Sign(instantRotation))
					? scene.parameters.GainsRotational.opposite - 1
					: scene.parameters.GainsRotational.same - 1);
			}
			return 0f;
		}

		public static float GetRedirectionReset(Scene scene) {
			float angleToTarget = scene.HeadToHeadRedirection.eulerAngles.y;
			float instantRotation = scene.GetHeadInstantRotationY();

			if (Mathf.Abs(angleToTarget) > scene.parameters.RotationalError && Mathf.Abs(instantRotation) > scene.parameters.RotationalThreshold) {
				return instantRotation * ((Mathf.Sign(angleToTarget) != Mathf.Sign(instantRotation))
					? scene.parameters.GainsRotational.opposite - 1
					: scene.parameters.GainsRotational.same - 1);
			}
			return 0f;
		}
	}

	/// <summary>
	/// This class implements the curvature technique from Razzaque et al., 2001. This technique rotates the user's virtual head around the vertical axis by an amount proportional
	/// to their linear speed in the opposite direction of the forward target. This is done in order to push the user to turn towards the target.
	/// </summary>
	public class Razzaque2001Curvature : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.RotateVirtualHeadY(GetRedirection(scene));
			CopyHeadAndLimbTransform(scene);
		}

		public static float GetRedirection(Scene scene) {
			float instantTranslation = scene.GetHeadInstantTranslationForward().magnitude;

			return instantTranslation > scene.parameters.WalkingThreshold * Time.deltaTime
				? Mathf.Sign(Vector3.Cross(scene.physicalHead.forward, scene.forwardTarget).y) * instantTranslation * CurvatureRadiusToRotationRate(scene)
				: 0f;
		}

		public static float CurvatureRadiusToRotationRate(Scene scene) => 180f / (Mathf.PI * scene.parameters.CurvatureRadius);
	}


	/// <summary>
	/// This class implements the complete Redirected Walking technique from Razzaque et al., 2001. This technique applies the maximum rotation obtained using:
	/// - the over time rotation technique
	/// - the rotationnal technqiue
	/// - the curvature technique
	/// to the user's head.
	/// </summary>
	public class Razzaque2001Hybrid : WorldRedirectionTechnique {

		public Func<float, float, float, float> aggregate = (a, b, c) => a + b + c;


		/// <summary>
		/// By default, the aggregation function is the sum of redirection components.
		/// </summary>
		public Razzaque2001Hybrid() : base() => Sum();

		/// <summary>
		/// Constructor taking a parameter, an aggregation function (float, float, float) -> float.
		/// </summary>
		/// <param name="aggregate"></param>
		public Razzaque2001Hybrid(Func<float, float, float, float> aggregate) : base() => this.aggregate = aggregate;

		/// <summary>
		/// Static factory method for using Maximum value aggregation.
		/// </summary>
		/// <param name="aggregate"></param>
		public static Razzaque2001Hybrid Max() => new((a, b, c) => (new float[] { a, b, c }).MinBy(Mathf.Abs));

		/// <summary>
		/// Static factory method for using sum-aggregation.
		/// </summary>
		/// <param name="aggregate"></param>
		public static Razzaque2001Hybrid Sum() => new((a, b, c) => a + b + c);

		/// <summary>
		/// Static factory method for using weighted-sum-aggregation.
		/// </summary>
		public static Razzaque2001Hybrid Weighted(float x, float y, float z) => new((a, b, c) => a * x + b * y + c * z);

		public override void Redirect(Scene scene) {
			float angle = aggregate(
				scene.enableHybridOverTime ? Razzaque2001OverTimeRotation.GetRedirection(scene) : 0,
				scene.enableHybridRotational ? Razzaque2001Rotational.GetRedirection(scene) : 0,
				scene.enableHybridCurvature ? Razzaque2001Curvature.GetRedirection(scene) : 0
			);

			if (scene.applyDampening) {
				angle = ApplyDampening(scene, angle);
			}
			if (scene.applySmoothing) {
				angle = ApplySmoothing(scene, angle);
			}

			scene.previousRedirection = angle;
			scene.RotateVirtualHeadY(angle);

			CopyHeadAndLimbTransform(scene);
		}

		public float GetRedirection(Scene scene) {
			return aggregate(
				Razzaque2001OverTimeRotation.GetRedirection(scene),
				Razzaque2001Rotational.GetRedirection(scene),
				Razzaque2001Curvature.GetRedirection(scene)
			);
		}

		private float ApplyDampening(Scene scene, float angle) {
			float dampenedAngle = angle * Mathf.Sin(Mathf.Min(scene.GetHeadAngleToTarget() / scene.parameters.DampeningRange, 1f) * Mathf.PI / 2);
			float dampenedAngleDistance = dampenedAngle * Mathf.Min(scene.GetHeadToTargetDistance() / scene.parameters.DampeningDistanceThreshold, 1f);
			return (scene.GetHeadToTargetDistance() < scene.parameters.DampeningDistanceThreshold) ? dampenedAngleDistance : dampenedAngle;
		}

		public float ApplySmoothing(Scene scene, float angle) => (1 - scene.parameters.SmoothingFactor) * scene.previousRedirection + scene.parameters.SmoothingFactor * angle;
	}

	/// <summary>
	/// This class implements the translationnal technique from Steinicke et al., 2008. This technique scales the user's displacement in order to virtually increase the space
	/// the user can explore freely.
	/// </summary>
	public class Steinicke2008Translational : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.virtualHead.Translate(
				Vector3.Scale(scene.GetHeadInstantTranslation(), scene.parameters.GainsTranslational - Vector3.one),
				relativeTo: Space.World);
			CopyHeadAndLimbTransform(scene);
		}
	}

	// TODO: fix with HeadToHeadRedirection
	/// <summary>
	/// This class implements the world warping technique from Azmandian et al., 2016. This technique applies a gain to the user's head rotation in order to co-localize a physical object
	/// and its virtual counterpart.
	/// </summary>
	public class Azmandian2016World : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.virtualHead.RotateAround(scene.origin.position, Vector3.up, GetRedirection(scene));
			CopyHeadAndLimbTransform(scene);
		}

		public static float GetRedirection(Scene scene) {
			float angleBetweenTargets = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalTarget.position - scene.origin.position, Vector3.up), scene.virtualTarget.position - scene.origin.position, Vector3.up);
			float angleBetweenHeads = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.virtualHead.forward, Vector3.up);

			if (Mathf.Abs(angleBetweenTargets - angleBetweenHeads) > scene.parameters.RotationalError) {
				float angle = angleBetweenTargets - angleBetweenHeads;
				float instantRotation = scene.GetHeadInstantRotationY();

				if (Mathf.Abs(instantRotation) > scene.parameters.RotationalThreshold && Mathf.Abs(angle) > scene.parameters.RotationalError) {
					var gain = (Mathf.Sign(angle) != Mathf.Sign(instantRotation)) ? scene.parameters.GainsRotational.same : scene.parameters.GainsRotational.opposite;
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
	public class ResetWorldRedirection : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) { }
	}

	public class NoWorldRedirection : WorldRedirectionTechnique {
		public override void Redirect(Scene scene) => CopyHeadAndLimbTransform(scene);
	}
}