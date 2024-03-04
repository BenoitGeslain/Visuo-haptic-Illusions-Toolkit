using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;
using CsvHelper.Configuration.Attributes;
using static VHToolkit.Future;

using UnityEngine;

namespace VHToolkit.Redirection {

	/// <summary>
	/// A Limb pairs one <c>physicalLimb</c> Transform with a list of virtual limb Transforms.
	/// </summary>
	[Serializable]
	public struct Limb {
        public Transform physicalLimb;
		public List<Transform> virtualLimb;

		public Limb(Transform physicalLimb, List<Transform> virtualLimb) {
			this.physicalLimb = physicalLimb;
			this.virtualLimb = virtualLimb;
		}
	}

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
        // [Ignore] public Transform physicalHand;
        // [Ignore] public Transform virtualHand;
        [SerializeField] public List<Limb> limbs;

        [Ignore] public List<Transform> virtualLimbs => limbs.SelectMany(limb => limb.virtualLimb).ToList();

        [Ignore] public Transform physicalHead;
		[Ignore] public Transform virtualHead;

        [Ignore] public Transform physicalTarget;

        [Ignore] public Transform virtualTarget;
        [Ignore] public Transform origin;
		[Ignore] public List<Transform> targets;
		[Ignore] public float steerToOrbitRadius = 5f;
		[Ignore] public bool applyDampening = false;
		[Ignore] public bool applySmoothing = false;

		[Ignore] public Transform selectedTarget;
		[Ignore] public Vector3 forwardTarget;

		[Ignore] public List<Vector3> previousLimbPositions;
		[Ignore] public List<Quaternion> previousLimbRotations;
		[Ignore] public Vector3 previousHeadPosition;
		[Ignore] public Quaternion previousHeadRotation;
		[Ignore] public float previousRedirection;
		[Ignore] public Vector3 strategyDirection;

		[Ignore] public ParametersToolkit parameters;

		/// <summary>
		/// The position of the virtual limb is given by <c>physicalHand.position + Redirection</c>.
		/// </summary>
		[Ignore] public List<Vector3> LimbRedirection {
            get => limbs.ConvertAll(limb => limb.virtualLimb[0].position - limb.physicalLimb.position);
            set {
				foreach ((var limb, var v) in limbs.Zip(value)) {
					limb.virtualLimb.ForEach(vLimb => vLimb.position = limb.physicalLimb.position + v);
				}
        	}
		}

        /// <returns>The distance between the user's real and virtual hands.</returns>
        public List<List<float>> GetHandRedirectionDistance() => limbs.ConvertAll(limb => limb.virtualLimb.ConvertAll(vlimb => Vector3.Distance(limb.physicalLimb.position, vlimb.position)));

		/// <returns>The distance between the user's real hand and the physical target.</returns>
		public List<float> GetPhysicalHandTargetDistance() => limbs.ConvertAll(limb => Vector3.Distance(limb.physicalLimb.position, physicalTarget.position));

		/// <returns>The distance between the user's physical hand and the origin.</returns>
		public List<float> GetPhysicalHandOriginDistance() => limbs.ConvertAll(limb => Vector3.Distance(limb.physicalLimb.position, origin.position));

		/// <returns>The distance between the user's physical and virtual head.</returns>
		public float GetHeadToHeadDistance() => Vector3.Distance(physicalHead.position, virtualHead.position);

		private Quaternion _redirection = Quaternion.identity;
        /// <returns>The quaternion rotation between the user's physical and virtual head.</returns>
        public Quaternion HeadToHeadRedirection {
			get => _redirection;
			set { virtualHead.rotation = value * physicalHead.rotation; _redirection = value; }
		}

        /// <returns>The distance between the user's physical head and the target.</returns>
        public float GetHeadToTargetDistance() => Vector3.Distance(physicalHead.position, selectedTarget.position);

		/// <returns>The signed angle (as per trigonometry standards) between the user's physical head and the forward target in degrees.</returns>
        public float GetHeadAngleToTarget() => Vector3.SignedAngle(Vector3.ProjectOnPlane(physicalHead.forward, Vector3.up), forwardTarget, Vector3.up);

        /// <returns>The physical head's rotation with respect to the last frame's rotation, as a quaternion.</returns>
        public Quaternion GetHeadInstantRotation() => physicalHead.rotation * Quaternion.Inverse(previousHeadRotation);

        /// <returns>The instant angular velocity around the up axis (Y) of the physical head using the last frame's position.</returns>
        public float GetHeadInstantRotationY() {
			// float instantRotation = (Quaternion.Inverse(physicalHead.rotation) * GetHeadInstantRotation() * physicalHead.rotation ).eulerAngles.y;
			float instantRotation = GetHeadInstantRotation().eulerAngles.y;
			return (instantRotation > 180f)? 360f - instantRotation : instantRotation;
		}

		/// <returns>The instant linear velocity of the physical head using the last frame's position</returns>
		public Vector3 GetHeadInstantTranslation() => physicalHead.position - previousHeadPosition;

		/// <returns>The instant linear velocity of the physical head using the last frame's position projected on the physical head forward vector.</returns>
		public Vector3 GetHeadInstantTranslationForward() => Vector3.Project(GetHeadInstantTranslation(), physicalHead.forward);

        /// <returns>The instant linear velocity of the physical hand using the last frame's position</returns>
        public List<Vector3> GetLimbInstantTranslation() => limbs.Zip(previousLimbPositions, (limb, pLimb) => limb.physicalLimb.position - pLimb).ToList();


        /// <summary>
        /// Applies unaltered physical head rotations to the virtual head GameObject
        /// </summary>
        public void CopyHeadRotations() => virtualHead.rotation = HeadToHeadRedirection * physicalHead.rotation;

        /// <summary>
        /// Applies unaltered physical head translations to the virtual head GameObject
        /// </summary>
        public void CopyHeadTranslations() => virtualHead.Translate(HeadToHeadRedirection * GetHeadInstantTranslation(), relativeTo: Space.World);

        /// <summary>
        /// Applies unaltered physical hand rotations to the virtual hand GameObjects
        /// </summary>
        public void CopyLimbTranslationAndRotation() {
			limbs.ForEach(limb => {
				Vector3 offset = physicalHead.InverseTransformPoint(limb.physicalLimb.position);
				foreach (var vlimb in limb.virtualLimb) {
					vlimb.SetPositionAndRotation(virtualHead.TransformPoint(offset), HeadToHeadRedirection * limb.physicalLimb.rotation);
                }
			});
		}


        /// <summary>
        /// Rotate the virtual head by the given angle (in degrees) around the world's y axis
        /// </summary>
        public void RotateVirtualHeadY(float angle) {
            HeadToHeadRedirection = Quaternion.Euler(0f, angle, 0f) * HeadToHeadRedirection;
		}
	}
}
