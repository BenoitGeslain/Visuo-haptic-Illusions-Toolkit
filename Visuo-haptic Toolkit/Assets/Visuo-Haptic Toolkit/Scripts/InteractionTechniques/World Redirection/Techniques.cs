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
		///
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
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				Debug.Log(Toolkit.Instance.parameters.OverTimeRotation*Time.deltaTime);
				virtualHead.Rotate(0f, Toolkit.Instance.parameters.OverTimeRotation*Time.deltaTime, 0f, Space.World);
			}
        }

        public float GetFrameOffset() => Toolkit.Instance.parameters.OverTimeRotation;
    }

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to her angular speed.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) < Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, GetFrameOffset(forwardTarget, physicalHead, virtualHead, previousPosition, previousOrientation), 0f);
			}
        }

		public float GetFrameOffset(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead, Vector3 previousPosition, Quaternion previousOrientation) {
			float instantRotation = physicalHead.eulerAngles.y - previousOrientation.eulerAngles.y;

			if (Mathf.Sign(Vector3.SignedAngle(physicalHead.forward, forwardTarget, Vector3.up)) == Mathf.Sign(instantRotation)) {
				return instantRotation * Toolkit.Instance.parameters.GainsRotational.same;
			}
			return instantRotation * Toolkit.Instance.parameters.GainsRotational.opposite;
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