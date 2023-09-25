using System;
using UnityEngine;

namespace BG.Redirection {
	[CreateAssetMenu(fileName = "Data", menuName = "VR Toolkit/Interaction Techniques Parameters", order = 1)]
	public class ParametersToolkit: ScriptableObject {

		[Header("Body Warping")]
		[Tooltip("The detection thresholds measured by Zenner et al., 2019 also called Just Noticeable Difference or Intervals of Non-detection. The coordinate system is that of Unity, i.e. X: Letf/Right, Y: Up/Down, Z: Forward/Backward. Values are in degrees.")]
		public Vector3 MaxAngles;
		[Tooltip("A buffer value where the hand redirection is not applied [Han et al., 2018].")]
		public float NoRedirectionBuffer = 0.1f;
		[Tooltip("A buffer value where the hand redirection is not applied [Han et al., 2018].")]
		public float MaxRedirectionThreshold = 0.5f;
		[Tooltip("Speed at which the hand redirection is reset when selecting the ResetRedirection technique. Value is in XXXX.")]
		public float ResetRedirectionSpeed = 0.001f;
		[Tooltip("")]
		public float GoGoCoefficient = 1f;
		[Tooltip("")]
		public float GoGoActivationDistance = 0.167f;

		[Header("World Warping")]
		[Tooltip("The error in rotation where users are considered to be in the correct direction. Value is in °.")]
		public float RotationalEpsilon = 0.5f;
		[Tooltip("The minimum head rotation required to apply a world redirection. Value is in °/s.")]
		public float MinimumRotation = 0f;
		[Tooltip("The speed threshold defining if the user is considered to be standing still or walking. Value is in m/s. [Hogson and Bachmann, 2013]")]
		public float WalkingThreshold = 0.2f;

		[Tooltip("The speed threshold defining if the user is considered to be standing still or walking. Value is in m/s. [Hogson and Bachmann, 2013]")]
		public float DistanceThreshold = 1.25f;
		[Tooltip("The dampening range. Value is in °.")]
		public float DampeningRange = 0f;
		[Range(0,1)]
		[Tooltip("The soothing factor preventing abrupt changes. Value has no unit. [Hogson and Bachmann, 2013]")]
		public float SmoothingFactor = 0.2f;

		[Tooltip("The maximum rotation that can be applied to the user's point of view in rotation along the vertical axis (Y). Value is in °/s.")]
		public float OverTimeRotation = 0.2f;
		[Tooltip("The maximum gain in translation that can be applied to the user's point of view in translation (akin to a C/D ratio). Value has no unit and is not a percentage.")]
		public Vector3 GainsTranslational;
		[Tooltip("The maximum gain in rotation that can be applied to the user's point of view in rotation. Value has no unit and is not a percentage. [Steinicke et al., 2010]")]
		public Vector2Rotation GainsRotational;
		[Tooltip("The maximum gain in translation that can be applied to the user's point of view in rotation. Value is in °/m and is not a percentage.")]
		public float CurvatureRadius = 7.5f;

		[Header("3D Interpolation")]
		[Tooltip("")]
		public Vector3 a;

		[Header("Pseudo-Haptic")]
		[Tooltip("")]
		public Vector3 b;
	}
}