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

		public virtual void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

	public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Steinicke2008Translational: WorldRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
			Debug.Log("Method not implemented yet.");
        }
	}

	public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(Transform physicalTarget, Transform virtualTarget, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }
	}
}