using UnityEngine;

namespace BG.Redirection {

    /// <summary>
    ///  This class is the most conceptual class of  world redirection defining the important
    ///  functions to call: Redirect()
    /// </summary>
    public class WorldRedirectionTechnique {

		/// <summary>
		/// Redirects the user towards the actual target. Should be overriden in subclasses.
		/// </summary>
		public virtual void Redirect(Scene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();
			scene.virtualHead.Rotate(0f, GetFrameOffset(scene), 0f, Space.World);
        }

        public static float GetFrameOffset(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();

            return Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon
                ? Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime
                : 0f;
        }
    }

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to their angular speed.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
		public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(GetFrameOffset(scene));
			scene.CopyHeadTranslations();
        }

		public static float GetFrameOffset(Scene scene) {
			float angleToTarget = scene.GetHeadAngleToTarget();
			float instantRotation = scene.GetHeadInstantRotationY();

			if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.MinimumRotation && Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				return instantRotation * ((Mathf.Sign(scene.GetHeadAngleToTarget()) == Mathf.Sign(instantRotation))
					? Toolkit.Instance.parameters.GainsRotational.opposite
					: Toolkit.Instance.parameters.GainsRotational.same);
			}
			return 0f;
		}
	}

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to their linear velocity.
	/// </summary>
	public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(GetFrameOffset(scene));
			scene.CopyHeadTranslations();
        }

		public static float GetFrameOffset(Scene scene) {
			float instantTranslation = scene.GetHeadInstantTranslationForward().magnitude;

            return instantTranslation > Toolkit.Instance.parameters.WalkingThreshold * Time.deltaTime
                ? Mathf.Sign(Vector3.Cross(scene.physicalHead.forward, scene.forwardTarget).y) * instantTranslation * Toolkit.Instance.CurvatureRadiusToRotationRate()
                : 0f;
        }
    }

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();

			Vector3 instantTranslation = scene.GetHeadInstantTranslation();
			Vector3 translation = new Vector3(instantTranslation.x * Toolkit.Instance.parameters.GainsTranslational.x,
											  instantTranslation.y * Toolkit.Instance.parameters.GainsTranslational.y,
											  instantTranslation.z * Toolkit.Instance.parameters.GainsTranslational.z);
			scene.virtualHead.position += translation;
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
            float angle = Mathf.Max(
				Razzaque2001OverTimeRotation.GetFrameOffset(scene),
				Razzaque2001Rotational.GetFrameOffset(scene),
				Razzaque2001Curvature.GetFrameOffset(scene)
			);

			if (scene.applyDampening) {
				angle = ApplyDampening(scene, angle);
			}
			if (scene.applySmoothing) {
				angle = ApplyDampening(scene, angle);
			}

			scene.previousRedirection = angle;

			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(angle);
			scene.CopyHeadTranslations();
        }

		public float ApplyDampening(Scene scene, float angle) {
			float dampenedAngle = angle * Mathf.Sin(Mathf.Min(scene.GetHeadAngleToTarget() / Toolkit.Instance.parameters.DampeningRange, 1f) * Mathf.PI/2);
			float dampenedAngleDistance = dampenedAngle * Mathf.Min(scene.GetHeadToTargetDistance() / Toolkit.Instance.parameters.DistanceThreshold, 1f);
			return (scene.GetHeadToTargetDistance() < Toolkit.Instance.parameters.DistanceThreshold)? dampenedAngleDistance : dampenedAngle;
		}

        public float ApplySmooting(Scene scene, float angle) => (1 - Toolkit.Instance.parameters.SmoothingFactor) * scene.previousRedirection + Toolkit.Instance.parameters.SmoothingFactor * angle;
    }

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();

			scene.virtualHead.RotateAround(scene.origin.position, Vector3.up, GetFrameOffset(scene));
        }

		public static float GetFrameOffset(Scene scene) {
			float angleBetweenTargets = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalTarget.position - scene.origin.position, Vector3.up), scene.virtualTarget.position - scene.origin.position, Vector3.up);
			float angleBetweenHeads = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.virtualHead.forward, Vector3.up);

			if (Mathf.Abs(angleBetweenTargets - angleBetweenHeads) > Toolkit.Instance.parameters.RotationalEpsilon) {
				float angle = angleBetweenTargets - angleBetweenHeads;
				float instantRotation = scene.GetHeadInstantRotationY();

				if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.MinimumRotation && Mathf.Abs(angle) > Toolkit.Instance.parameters.RotationalEpsilon) {
					var gain = (Mathf.Sign(angle) == Mathf.Sign(instantRotation)) ? Toolkit.Instance.parameters.GainsRotational.same : Toolkit.Instance.parameters.GainsRotational.opposite;
					var bound = Mathf.Abs(gain * instantRotation);
					return Mathf.Clamp(angle, -bound, bound);
				}
			}
			return 0f;
		}
	}

	public class ResetWorldRedirection: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
            Debug.Log("Method not implemented yet.");
        }
    }
}