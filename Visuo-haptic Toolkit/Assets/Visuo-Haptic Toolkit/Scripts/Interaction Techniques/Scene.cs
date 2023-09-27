using System;

using UnityEngine;

namespace BG.Redirection {
	/// <summary>
	/// This class records the position of various objects of interest.
	/// </summary>
	/// <param name="physicalHand">The Transform representing the user's physical hand, i.e. the transform that is receiving the tracked hand data.</param>
	/// <param name="virtualHand">The Transform representing the user's virtual hand, i.e. the user's avatar.</param>
	/// <param name="physicalHead">The Transform representing the user's physical head, i.e. the transform that is receiving the tracked head data..</param>
	/// <param name="virtualHead">The Transform representing the user's virtual hand.</param>
	///
	/// <param name="physicalTarget">The physical target the hand should reach when the virtual hand reaches the virtual target.</param>
	/// <param name="virtualTarget">The virtual target the user is reaching for.</param>
	/// <param name="origin">The point of origin where no redirection is applied.</param>
	/// <param name="targets">The target(s) used for the steering strategies. For SteerToOrbit, the first element is considered to be the center of the orbit.</param>
	/// <param name="radius">The radius of the orbit in the SteerToOrbit strategy.</param>
	/// <param name="applyDampening">Whether to apply dampening in the Razzaque et al.'s Redirected walking hybrid technique.</param>
	/// <param name="applySmoothing">Whether to apply smoothing in the Razzaque et al.'s Redirected walking hybrid technique.</param>
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
		[HideInInspector] public Vector3 previousHandPosition;
		[HideInInspector] public Quaternion previousHandRotation;
		[HideInInspector] public Vector3 previousHeadPosition;
		[HideInInspector] public Quaternion previousHeadRotation;
		[HideInInspector] public float previousRedirection;

        /// <summary>
        /// The position of the virtual hand is given by <c>physicalHand.position + Redirection</c>.
        /// </summary>
        public Vector3 Redirection {
			get => virtualHand.position - physicalHand.position;
			set => virtualHand.position = physicalHand.position + value;
		}

        /// <summary>
        ///
        /// </summary>
        /// <returns>Returns the distance between the user's real and virtual hands.</returns>
        public float GetHandRedirectionDistance() => Vector3.Distance(physicalHand.position, virtualHand.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the distance between the user's real hand and the physical target.</returns>
		public float GetPhysicalHandTargetDistance() => Vector3.Distance(physicalHand.position, physicalTarget.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the distance between the user's physical hand and the origin.</returns>
		public float GetPhysicalHandOriginDistance() => Vector3.Distance(physicalHand.position, origin.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the distance between the user's physical and virtual head.</returns>
		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the quaternion rotation between the user's physical and virtual head.</returns>
		public Quaternion GetHeadToHeadRotation() => Quaternion.FromToRotation(physicalHead.forward, virtualHead.forward);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the distance between the user's physical head and the target.</returns>
		public float GetHeadToTargetDistance() => Vector3.Distance(physicalHead.position, selectedTarget.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the signed angle (as per trigonometry standards) between the user's physical head and the forward target in degrees.</returns>
        public float GetHeadAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the instant angular velocity as a quaternion of the physical head using the last frame's rotation</returns>
        public Quaternion GetHeadInstantRotation() {
			return physicalHead.rotation * Quaternion.Inverse(previousHeadRotation);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the instant angular velocity around the up axis (Y) of the physical head using the last frame's position</returns>
        public float GetHeadInstantRotationY() {
			float instantRotation = GetHeadInstantRotation().eulerAngles.y;
			return (instantRotation > 180f)? 360 - instantRotation : instantRotation;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the instant linear velocity of the physical head using the last frame's position</returns>
		public Vector3 GetHeadInstantTranslation() => physicalHead.position - previousHeadPosition;

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the instant linear velocity of the physical head using the last frame's position projected on the physical head forward vector.</returns>
		public Vector3 GetHeadInstantTranslationForward() => Vector3.Project(GetHeadInstantTranslation(), physicalHead.forward);

		/// <summary>
		///
		/// </summary>
		/// <returns>Returns the instant linear velocity of the physical hand using the last frame's position</returns>
		public Vector3 GetHandInstantTranslation() => physicalHand.position - previousHandPosition;

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadRotations() {
            virtualHead.rotation = physicalHead.rotation * Quaternion.Inverse(previousHeadRotation) * virtualHead.rotation;
		}

        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadTranslations() {
            virtualHead.position += physicalHead.position - previousHeadPosition;
		}

		/// <summary>
		/// Rotate the virtual head by the given amount of degrees around the world's y axis
		/// </summary>
		public void RotateVirtualHeadY(float degrees) {
            virtualHead.Rotate(xAngle: 0f, yAngle: degrees, zAngle: 0f, relativeTo: Space.World);
        }
    }
}