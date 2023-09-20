using TMPro;
using UnityEditor;
using UnityEngine;

namespace BG.Redirection {

	public record WorldRedirectionScene()
	{
        [Header("User Parameters")]
        public Transform physicalHead;
		public Transform virtualHead;
        [Header("Technique Parameters")]
        public Vector3 forwardTarget;
		public Vector3 previousPosition; // TODO set private
		public Quaternion previousRotation;
		public Transform[] targets;
		public float radius;

		public float previousRedirection;

		public WorldRedirectionScene(Transform physicalHead, Transform virtualHead, Vector3 forwardTarget) : this()
		{
            this.physicalHead = physicalHead;
			this.virtualHead = virtualHead;
			this.forwardTarget = forwardTarget;
			this.previousPosition = physicalHead.position;
			this.previousRotation = physicalHead.rotation;
		}

		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);
		public float GetDistanceToTarget() => Vector3.Distance(physicalHead.position, targets[0].position);	// TODO Implement target selectin in Strategy
        public float GetAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);
        public float GetInstantaneousRotation() => physicalHead.eulerAngles.y - previousRotation.eulerAngles.y;

		public Vector3 GetInstantaneousTranslation() => Vector3.Project(physicalHead.position - previousPosition, physicalHead.forward);

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadRotations() {
            virtualHead.rotation = physicalHead.rotation * Quaternion.Inverse(previousRotation) * virtualHead.rotation;
		}

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadTranslations() {
            virtualHead.position += physicalHead.position - previousPosition;
		}

		/// <summary>
		/// Rotate the virtual head by the given amount of degrees around the world's y axis
		/// </summary>
		public void RotateVirtualHeadY(float degrees)
		{
            virtualHead.Rotate(xAngle: 0f, yAngle: degrees, zAngle: 0f, relativeTo: Space.World);
        }
    }

    /// <summary>
    ///  This class is the most conceptual class of  world redirection defining the important
    ///  functions to call: Redirect()
    /// </summary>
    public class WorldRedirectionTechnique {

		public virtual void InitRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		/// <summary>
		/// Redirects the user towards the actual target. Should be overriden in subclasses.
		/// </summary>
		public virtual void Redirect(WorldRedirectionScene scene) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
			scene.CopyHeadRotations();
			scene.CopyHeadTranslations();
			scene.virtualHead.Rotate(0f, GetFrameOffset(scene), 0f, Space.World);
        }

        public static float GetFrameOffset(WorldRedirectionScene scene) {
			float angleToTarget = scene.GetAngleToTarget();

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
		public override void Redirect(WorldRedirectionScene scene) {
			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(GetFrameOffset(scene));
			scene.CopyHeadTranslations();
        }

		public static float GetFrameOffset(WorldRedirectionScene scene) {
			float angleToTarget = scene.GetAngleToTarget();
			float instantRotation = scene.GetInstantaneousRotation();

			if (Mathf.Abs(instantRotation) > Toolkit.Instance.parameters.MinimumRotation && Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				return instantRotation * ((Mathf.Sign(scene.GetAngleToTarget()) == Mathf.Sign(instantRotation))
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
        public override void Redirect(WorldRedirectionScene scene) {
			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(GetFrameOffset(scene));
			scene.CopyHeadTranslations();
        }

		public static float GetFrameOffset(WorldRedirectionScene scene) {
			float angleToTarget = scene.GetAngleToTarget();
			float instantTranslation = scene.GetInstantaneousTranslation().magnitude;

			if (instantTranslation > Toolkit.Instance.parameters.WalkingThreshold) {
				return Mathf.Sign(angleToTarget) * instantTranslation * Toolkit.Instance.CurvatureRadiusToRotationRate();
			}
			return 0f;
		}
	}

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            float angle = Mathf.Max(
				Razzaque2001OverTimeRotation.GetFrameOffset(scene),
				Razzaque2001Rotational.GetFrameOffset(scene),
				Razzaque2001Curvature.GetFrameOffset(scene)
			);
			scene.previousRedirection = angle;

			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(angle);
			scene.CopyHeadTranslations();
        }

		public float GetDampenedFrameOffset(WorldRedirectionScene scene) {
			float dampened = GetDampenedFrameOffset(scene) * Mathf.Sin(Mathf.Min(scene.GetAngleToTarget() / Toolkit.Instance.parameters.DampeningRange, 1f) * Mathf.PI/2);
			// TODO replace 1f by distance to target
			return (scene.GetDistanceToTarget() < Toolkit.Instance.parameters.DistanceThreshold)? dampened * Mathf.Min(scene.GetDistanceToTarget() / Toolkit.Instance.parameters.DistanceThreshold, 1f) : dampened;
		}

        public float GetSmoothedFrameOffset(WorldRedirectionScene scene) => (1 - Toolkit.Instance.parameters.SmoothingFactor) * scene.previousRedirection + Toolkit.Instance.parameters.SmoothingFactor * GetDampenedFrameOffset(scene);
    }

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class ResetWorldRedirection: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            Debug.Log("Method not implemented yet.");
        }
    }
}