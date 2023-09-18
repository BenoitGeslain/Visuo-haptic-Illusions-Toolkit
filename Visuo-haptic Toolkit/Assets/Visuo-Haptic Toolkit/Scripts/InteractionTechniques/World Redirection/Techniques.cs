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

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void copyHeadRotations() {
			Quaternion q = this.physicalHead.rotation * Quaternion.Inverse(this.previousRotation);
			this.virtualHead.rotation = q * this.virtualHead.rotation;
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
			float angleToTarget = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.forwardTarget, Vector3.up);

			// Debug.Log(angleToTarget);
            if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
				scene.virtualHead.Rotate(0f, GetFrameOffset(angleToTarget), 0f, Space.World);
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
			float angleToTarget = Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.forwardTarget, Vector3.up);
			float instantaneousRotation = scene.physicalHead.eulerAngles.y - scene.previousRotation.eulerAngles.y;

			if (Mathf.Abs(instantaneousRotation) > Toolkit.Instance.parameters.MinimumRotation) {
				if (Mathf.Abs(angleToTarget) > Toolkit.Instance.parameters.RotationalEpsilon) {
					scene.virtualHead.Rotate(0f, GetFrameOffset(scene), 0f);
				}
			} else {
				scene.copyHeadRotations();
			}
        }

		public float GetFrameOffset(WorldRedirectionScene scene) {
			float instantaneousRotation = scene.physicalHead.eulerAngles.y - scene.previousRotation.eulerAngles.y;

            return instantaneousRotation *  Mathf.Sign(Vector3.SignedAngle(Vector3.ProjectOnPlane(scene.physicalHead.forward, Vector3.up), scene.forwardTarget, Vector3.up)) == Mathf.Sign(instantaneousRotation)
                ? Toolkit.Instance.parameters.GainsRotational.same
                : Toolkit.Instance.parameters.GainsRotational.opposite;
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