using TMPro;
using UnityEditor;
using UnityEngine;

namespace BG.Redirection {

	public record WorldRedirectionScene()
	{
		public Transform physicalHead;
		public Transform virtualHead;
		public Vector3 forwardTarget;
		public Vector3 previousPosition;
		public Quaternion previousRotation;

		public WorldRedirectionScene(Transform physicalHead, Transform virtualHead, Vector3 forwardTarget) : this()
		{
			this.physicalHead = physicalHead;
			this.virtualHead = virtualHead;
			this.forwardTarget = forwardTarget;
			this.previousPosition = physicalHead.position;
			this.previousRotation = physicalHead.rotation;
		}

		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);
        public float GetAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);
        public float GetInstantaneousRotation() => physicalHead.eulerAngles.y - previousRotation.eulerAngles.y;

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadRotations() {
            virtualHead.rotation = physicalHead.rotation * Quaternion.Inverse(previousRotation) * virtualHead.rotation;
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
        }

		public static float GetFrameOffset(WorldRedirectionScene scene) {
			float angleToTarget = scene.GetAngleToTarget();
			float instantTranslation = Vector3.Project(scene.physicalHead.position - scene.previousPosition, scene.physicalHead.forward).magnitude;

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

			scene.CopyHeadRotations();
			scene.RotateVirtualHeadY(angle);
        }
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