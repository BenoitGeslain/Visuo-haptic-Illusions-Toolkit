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
		public virtual void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) < Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, Toolkit.Instance.parameters.OverTimeRotaton, 0f);
			}
        }

		public float getFrameOffset() {
			return Toolkit.Instance.parameters.OverTimeRotaton;
		}
	}

	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) < Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, getFrameOffset(forwardTarget, physicalHead, virtualHead), 0f);
			}
        }

		public float getFrameOffset(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
			return 0;
		}
	}

	public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
			Debug.Log("Method not implemented yet.");
        }

		public void getFrameOffset() {

		}
	}

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}
}