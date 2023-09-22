using System;

using UnityEngine;

namespace BG.Redirection {
	/// <summary>
	/// This class records the position of various objects of interest.
	/// </summary>
	/// <param name="physicalTarget">The physical target the hand should reach when the virtual hand reaches the virtual target.</param>
	/// <param name="virtualTarget">The virtual target the user is reaching for.</param>
	/// <param name="origin">The point of origin where no redirection is applied.</param>
	/// <param name="physicalHand">The Transform representing the user's physical hand.</param>
	/// <param name="virtualHand">The Transform representing the user's virtual hand, i.e. the user's avatar.</param>
	[Serializable]
	public record Scene() {
        [Header("User Parameters")]
        public Transform physicalHand;
        public Transform virtualHand;
        public Transform physicalHead;
		public Transform virtualHead;

		[Header("Technique Parameters")]
        public Transform physicalTarget;
        public Transform virtualTarget;
        public Transform origin;
		public Transform[] targets;
		public float radius = 5f;
		public bool applyDampening = false;
		public bool applySmoothing = false;

		[HideInInspector] public Transform selectedTarget;
		[HideInInspector] public Vector3 forwardTarget;
		[HideInInspector] public Vector3 previousPosition;
		[HideInInspector] public Quaternion previousRotation;
		[HideInInspector] public float previousRedirection;

        public Scene(Transform physicalTarget, Transform virtualTarget, Transform origin, Transform physicalHand, Transform virtualHand):
			this() {

            this.physicalTarget = physicalTarget;
            this.virtualTarget = virtualTarget;
            this.origin = origin;
            this.physicalHand = physicalHand;
            this.virtualHand = virtualHand;
        }

        /// <summary>
        /// Distance between the user's real and virtual hands.
        /// </summary>
        /// <returns></returns>
        public float GetHandRedirectionDistance() => Vector3.Distance(physicalHand.position, virtualHand.position);

		/// <summary>
		/// Distance between the user's real hand and the physical target.
		/// </summary>
		/// <returns></returns>
		public float GetPhysicalHandTargetDistance() => Vector3.Distance(physicalHand.position, physicalTarget.position);

		/// <summary>
		/// Distance between the user's real hand and the origin.
		/// </summary>
		/// <returns></returns>
		public float GetPhysicalHandOriginDistance() => Vector3.Distance(physicalHand.position, origin.position);

        /// <summary>
        /// The position of the virtual hand is given by <c>physicalHand.position + Redirection</c>.
        /// </summary>
        public Vector3 Redirection {
			get => virtualHand.position - physicalHand.position;
			set => virtualHand.position = physicalHand.position + value;
		}

		// World Redir functions
		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);
		public float GetHeadRedirectionDistance() => Vector3.Distance(physicalHead.position, selectedTarget.position);	// TODO Implement target selectin in Strategy
        public float GetHeadAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up); // TODO Rendre 2 dimensions
        public float GetHeadInstantRotation() {
			float angularSpeed = (physicalHead.rotation * Quaternion.Inverse(previousRotation)).eulerAngles.y;
			return (angularSpeed>180f)? 360 - angularSpeed : angularSpeed;
		}

		public Vector3 GetHeadInstantTranslation() => Vector3.Project(physicalHead.position - previousPosition, physicalHead.forward);

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
		public void RotateVirtualHeadY(float degrees) {
            virtualHead.Rotate(xAngle: 0f, yAngle: degrees, zAngle: 0f, relativeTo: Space.World);
        }
    }
}