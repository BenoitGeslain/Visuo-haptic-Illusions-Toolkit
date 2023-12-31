using System;
using CsvHelper.Configuration.Attributes;

using UnityEngine;
using UnityEngine.Assertions;

namespace VHToolkit.Redirection {
	/// <summary>
	/// This class records the position of various objects of interest.
	/// </summary>
	/// <param name="physicalHand">The Transform representing the user's physical hand, i.e. the transform that is receiving the tracked hand data.</param>
	/// <param name="virtualHand">The Transform representing the user's virtual hand, i.e. the user's avatar.</param>
	/// <param name="physicalHead">The Transform representing the user's physical head, i.e. the transform that is receiving the tracked head data..</param>
	/// <param name="virtualHead">The Transform representing the user's virtual head.</param>
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
        [Ignore] public Transform physicalHand;
        [Ignore] public Transform virtualHand;
        [Ignore] public Transform physicalHead;
		[Ignore] public Transform virtualHead;

		[Header("Technique Parameters")]
        [Ignore] public Transform physicalTarget;
        [Ignore] public Transform virtualTarget;
        [Ignore] public Transform origin;
		[Ignore] public Transform[] targets;
		[Ignore] public float radius = 5f;
		[Ignore] public bool applyDampening = false;
		[Ignore] public bool applySmoothing = false;

		[Ignore] [HideInInspector] public Transform selectedTarget;
		[Ignore] [HideInInspector] public Vector3 forwardTarget;
		[Ignore] [HideInInspector] public Vector3 previousHandPosition;
		[Ignore] [HideInInspector] public Quaternion previousHandRotation;
		[Ignore] [HideInInspector] public Vector3 previousHeadPosition;
		[Ignore] [HideInInspector] public Quaternion previousHeadRotation;
		[Ignore] [HideInInspector] public float previousRedirection;

        /// <summary>
        /// The position of the virtual hand is given by <c>physicalHand.position + Redirection</c>.
        /// </summary>
        [Ignore] public Vector3 Redirection {
			get => virtualHand.position - physicalHand.position;
			set => virtualHand.position = physicalHand.position + value;
		}

        /// <returns>The distance between the user's real and virtual hands.</returns>
        public float GetHandRedirectionDistance() => Vector3.Distance(physicalHand.position, virtualHand.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>The distance between the user's real hand and the physical target.</returns>
		public float GetPhysicalHandTargetDistance() => Vector3.Distance(physicalHand.position, physicalTarget.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>The distance between the user's physical hand and the origin.</returns>
		public float GetPhysicalHandOriginDistance() => Vector3.Distance(physicalHand.position, origin.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>The distance between the user's physical and virtual head.</returns>
		public Vector3 GetHeadToHeadTranslation() => virtualHead.position - physicalHead.position;

		/// <summary>
		///
		/// </summary>
		/// <returns>The distance between the user's physical and virtual head.</returns>
		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);

        /// <summary>
        ///
        /// </summary>
        /// <returns>The quaternion rotation between the user's physical and virtual head.</returns>
        public Quaternion GetHeadToHeadRotation() => virtualHead.rotation * Quaternion.Inverse(physicalHead.rotation);

        /// <summary>
        ///
        /// </summary>
        /// <returns>The distance between the user's physical head and the target.</returns>
        public float GetHeadToTargetDistance() => Vector3.Distance(physicalHead.position, selectedTarget.position);

		/// <summary>
		///
		/// </summary>
		/// <returns>The signed angle (as per trigonometry standards) between the user's physical head and the forward target in degrees.</returns>
        public float GetHeadAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);

        /// <summary>
        ///
        /// </summary>
        /// <returns>The physical head's rotation with respect to the last frame's rotation, as a quaternion.</returns>
        public Quaternion GetHeadInstantRotation() => physicalHead.rotation * Quaternion.Inverse(previousHeadRotation);

        /// <summary>
        ///
        /// </summary>
        /// <returns>The instant angular velocity around the up axis (Y) of the physical head using the last frame's position.</returns>
        public float GetHeadInstantRotationY() {
			var Q = GetHeadToHeadRotation();
			float instantRotation = (Quaternion.Inverse(physicalHead.rotation)* GetHeadInstantRotation() *  physicalHead.rotation ).eulerAngles.y;
			return (instantRotation > 180f)? 360 - instantRotation : instantRotation;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The instant linear velocity of the physical head using the last frame's position</returns>
		public Vector3 GetHeadInstantTranslation() => physicalHead.position - previousHeadPosition;

		/// <returns>The instant linear velocity of the physical head using the last frame's position projected on the physical head forward vector.</returns>
		public Vector3 GetHeadInstantTranslationForward() => Vector3.Project(GetHeadInstantTranslation(), physicalHead.forward);

		/// <summary>
		///
		/// </summary>
		/// <returns>The instant linear velocity of the physical hand using the last frame's position</returns>
		public Vector3 GetHandInstantTranslation() => physicalHand.position - previousHandPosition;


        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadRotations() {
			var Q = GetHeadToHeadRotation();
            virtualHead.rotation = Q  * physicalHead.rotation * Quaternion.Inverse(previousHeadRotation) * Quaternion.Inverse(Q) * virtualHead.rotation;
		}

        /// <summary>
        /// Applies unaltered physical head translations to the virtual head GameObject
        /// </summary>
        public void CopyHeadTranslations() {
            virtualHead.position += GetHeadToHeadRotation() * GetHeadInstantTranslation();
            // virtualHead.Translate(GetHeadToHeadRotation() * GetHeadInstantTranslation(), relativeTo: Space.World);
		}

		/// <summary>
		/// Rotate the virtual head by the given angle (in degrees) around the world's y axis
		/// </summary>
		public void RotateVirtualHeadY(float angle) {
            virtualHead.Rotate(xAngle: 0f, yAngle: angle, zAngle: 0f, relativeTo: Space.World);
        }
    }
}