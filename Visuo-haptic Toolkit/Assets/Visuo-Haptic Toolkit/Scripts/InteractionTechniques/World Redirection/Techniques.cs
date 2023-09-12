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
		public virtual void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead) {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}

		public virtual void EndRedirection() {
			Debug.LogError("Calling Redirect() virtual method. It should be overriden");
		}
	}

    /// <summary>
    /// Class for rotating the world around the user by a fixed amount.
    /// </summary>
    public class Razzaque2001OverTimeRotation: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) < Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, Toolkit.Instance.parameters.OverTimeRotation, 0f);
			}
        }

        public float getFrameOffset() => Toolkit.Instance.parameters.OverTimeRotation;
    }

	/// <summary>
	/// Class for rotating the world around the user by an amount proportional to her angular speed.
	/// </summary>
	public class Razzaque2001Rotational: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead) {
            if (Vector3.Angle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget) < Toolkit.Instance.parameters.RotationalEpsilon) {
				virtualHead.Rotate(0f, getFrameOffset(forwardTarget, previousFrameYOrientation, physicalHead, virtualHead), 0f);
			}
        }

		public float getFrameOffset(Vector3 forwardTarget, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead) {
			float rotationRate = physicalHead.eulerAngles.y - previousFrameYOrientation;
			if (rotationRate > 0f) {
				return rotationRate * Toolkit.Instance.parameters.GainsRotational.right;
			}
			return rotationRate * Toolkit.Instance.parameters.GainsRotational.left;
		}
	}

    /// <summary>
    /// Class for rotating the world around the user by an amount proportional to her velocity.
    /// </summary>
    public class Razzaque2001Curvature: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead) {
            Debug.Log("Method not implemented yet.");
        }

		public void getFrameOffset() {

		}
	}

    /// <summary>
    /// Class for applying a translational gain to the user's movement.
    /// </summary>
    public class Steinicke2008Translational: WorldRedirectionTechnique {
		float gain = 2f; // TODO manage gain
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead)
        {
			// TODO: target prediction, returning if it fails
			virtualHead.position += (physicalHead.position - previousPosition) * gain;
            Debug.Log("Method not implemented yet.");
        }
	}

    /// <summary>
    /// Class for rotating the world around the user by the maximum of a constant amount, 
	/// an amount proportional to her angular speed and an amount proportional to her velocity.
    /// </summary>
    public class Razzaque2001Hybrid: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead)
        {

            Debug.Log("Method not implemented yet.");
        }
	}

	public class Azmandian2016World: WorldRedirectionTechnique {
        public override void Redirect(Vector3 forwardTarget, Vector3 previousPosition, float previousFrameYOrientation, Transform physicalHead, Transform virtualHead)
        {

            Debug.Log("Method not implemented yet.");
        }
    }
}