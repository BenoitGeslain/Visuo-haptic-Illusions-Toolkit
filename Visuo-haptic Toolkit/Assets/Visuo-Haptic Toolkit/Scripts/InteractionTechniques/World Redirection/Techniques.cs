using UnityEngine;

namespace BG.Redirection {

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
		/// <param name="forwardTarget">MUST be colinear with horizontal plane</param>
		/// <param name="physicalHead"></param>
		/// <param name="virtualHead"></param>
		public virtual void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		/// <summary>
		/// Applies unaltered physical head rotations to the virtual head GameObject
		/// </summary>
		/// <param name="physicalHead"></param>
		/// <param name="virtualHead"></param>
		/// <param name="previousOrientation"></param>
		protected void copyHeadRotations(Transform physicalHead, Transform virtualHead, Quaternion previousOrientation) {
			Quaternion q = physicalHead.rotation * Quaternion.Inverse(previousOrientation);
			virtualHead.rotation = q * virtualHead.rotation;
		}
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			float angleToTarget = Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);

			// Debug.Log(angleToTarget);
            if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, GetFrameOffset(angleToTarget), 0f, Space.World);
			}
			copyHeadRotations(physicalHead, virtualHead, previousOrientation);
        }

        public float GetFrameOffset(float angleToTarget) => Mathf.Sign(angleToTarget) * Toolkit.Instance.parameters.OverTimeRotation * Time.deltaTime;
    }

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to her angular speed.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			float angleToTarget = Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);
			float instantaneousRotation = physicalHead.eulerAngles.y - previousOrientation.eulerAngles.y;

			if (Mathf.Abs(instantaneousRotation) > Toolkit.Instance.parameters.MinimumRotation) {
				if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
					virtualHead.Rotate(0f, GetFrameOffset(forwardTarget, physicalHead, virtualHead, previousPosition, previousOrientation), 0f);
				}
			} else {
				copyHeadRotations(physicalHead, virtualHead, previousOrientation);
			}
        }

		public float GetFrameOffset(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			float instantaneousRotation = physicalHead.eulerAngles.y - previousOrientation.eulerAngles.y;

			if (Mathf.Sign(Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up)) == Mathf.Sign(instantaneousRotation)) {
				return instantaneousRotation * Toolkit.Instance.parameters.GainsRotational.same;
			}
			return instantaneousRotation * Toolkit.Instance.parameters.GainsRotational.opposite;
		}
	}

	public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			Debug.Log("Method not implemented yet.");
        }

		public void GetFrameOffset() {

		}
	}

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            Debug.Log("Method not implemented yet.");
        }
	}


	public class ResetWorldRedirection: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            Debug.Log("Method not implemented yet.");
        }
    }
}