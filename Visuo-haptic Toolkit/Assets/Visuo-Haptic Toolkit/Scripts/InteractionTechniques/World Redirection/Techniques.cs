using TMPro;
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
        public void copyHeadRotations() {
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
			float angleToTarget = scene.GetAngleToTarget();
            if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				scene.RotateVirtualHeadY(GetFrameOffset(angleToTarget));
			}
			scene.copyHeadRotations();
        }

        public float GetFrameOffset(float angleToTarget) => Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime;
    }

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to her angular speed.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {

            if (Mathf.Abs(scene.GetInstantaneousRotation()) > Toolkit.Instance.parameters.MinimumRotation) {
				if (Mathf.Abs(scene.GetAngleToTarget()) > Toolkit.Instance.parameters.RotationalEpsilon) {
                    scene.RotateVirtualHeadY(GetFrameOffset(scene));
                }
			} else {
				scene.copyHeadRotations();
			}
        }

		public float GetFrameOffset(WorldRedirectionScene scene) {
			float instantaneousRotation = scene.GetInstantaneousRotation();

            return instantaneousRotation * ((Mathf.Sign(scene.GetAngleToTarget()) == Mathf.Sign(instantaneousRotation))
                ? Toolkit.Instance.parameters.GainsRotational.same
                : Toolkit.Instance.parameters.GainsRotational.opposite);
        }
    }

	public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
			Debug.Log("Method not implemented yet.");
        }

		public void GetFrameOffset() {

		}
	}

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(WorldRedirectionScene scene) {
            Debug.Log("Method not implemented yet.");
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