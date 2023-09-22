using System;
using TMPro;
using UnityEditor;
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

            if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				return Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime;
			}
			return 0f;
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
			float instantRotation = scene.GetHeadInstantRotation();

			if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.MinimumRotation && Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				return instantRotation * ((Mathf.Sign(scene.GetHeadAngleToTarget()) == Mathf.Sign(instantRotation))
					? Toolkit.Instance.parameters.GainsRotational.same
					: Toolkit.Instance.parameters.GainsRotational.opposite);
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
			float angleToTarget = scene.GetHeadAngleToTarget();
			float instantTranslation = scene.GetHeadInstantTranslation().magnitude;

            return instantTranslation > Toolkit.Instance.parameters.WalkingThreshold
                ? Mathf.Sign(angleToTarget) * instantTranslation * Toolkit.Instance.CurvatureRadiusToRotationRate()
                : 0f;
        }
    }

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
            Debug.Log("Method not implemented yet.");
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
			float dampenedAngleDistance = dampenedAngle * Mathf.Min(scene.GetHeadRedirectionDistance() / Toolkit.Instance.parameters.DistanceThreshold, 1f);
			return (scene.GetHeadRedirectionDistance() < Toolkit.Instance.parameters.DistanceThreshold)? dampenedAngleDistance : dampenedAngle;
		}

        public float ApplySmooting(Scene scene, float angle) => (1 - Toolkit.Instance.parameters.SmoothingFactor) * scene.previousRedirection + Toolkit.Instance.parameters.SmoothingFactor * angle;
    }

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class ResetWorldRedirection: WorldRedirectionTechnique {
        public override void Redirect(Scene scene) {
            Debug.Log("Method not implemented yet.");
        }
    }
}