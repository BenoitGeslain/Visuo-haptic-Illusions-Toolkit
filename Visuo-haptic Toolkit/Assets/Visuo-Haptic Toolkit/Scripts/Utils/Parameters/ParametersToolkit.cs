using System;

using UnityEngine;

namespace VHToolkit.Redirection {
	[CreateAssetMenu(fileName = "Data", menuName = "VR Illusions Toolkit/Interaction Techniques Parameters", order = 1)]
	public class ParametersToolkit: ScriptableObject {

		[Header("Body Warping")]
		[Tooltip("The thresholds for the left and right direction. These values are used to show whether the targets or the hands are further apart than these values.\n\nThe recommended values are [-4.38°, 3.81°] from the measures made by Zenner et al., 2019 using the 75% detection rate. They are also called Just Noticeable Difference or Intervals of Non-detection.\n\nThis wiki page gives more details and a link to the paper referenced: Anonymized. Values are in degrees.")]
		public Vector2Horizontal HorizontalAngles;
		[Tooltip("The thresholds for the up and down direction. These values are used to show whether the targets or the hands are further apart than these value.\nThe recommended values are [] from the measures made by Zenner et al., 2019 using the 75% detection rate. They are also called Just Noticeable Difference or Intervals of Non-detection.\nThis wiki page gives more details and a link to the paper referenced: Anonimized. Values are in degrees.")]
		public Vector2Vertical VerticalAngles;
		[Tooltip("The thresholds for the forward (faster) and backward (slower) direction. These values are used to show whether the targets or the hands are further apart than these value.\nThe recommended values are [] from the measures made by Zenner et al., 2019 using the 75% detection rate. They are also called Just Noticeable Difference or Intervals of Non-detection.\nThis wiki page gives more details and a link to the paper referenced: Anonimized. Values are unitless.")]
		public Vector2Gain Gain;
		[Tooltip("A buffer value where the hand redirection is not applied [Han et al., 2018].")]
		public float RedirectionBuffer = 0.1f;
		[Tooltip("A buffer value where the hand redirection is not applied [Han et al., 2018].")]
		public float MaxRedirectionThreshold = 0.5f;	// what is this?
		[Tooltip("This coefficient is applied to the PoupyrevGoGo1996 \"redirection\"")]
		public float GoGoCoefficient = 1f;
		[Tooltip("The activation distance  for which the PoupyrevGoGo1996 technique starts to remap the virtual hand's movement")]
		public float GoGoActivationDistance = 0.167f;
		[Tooltip("Coefficient that controls the amount of redirection to remove according to the real hand translation when selecting the ResetRedirection technique. Value is in XXXX.")]	// TODO check this
		public float ResetRedirectionCoeff = 0.087f;



		[Header("World Warping")]
		[Tooltip("The error in rotation where users are considered to be in the correct direction. Value is in °.")]
		public float RotationalError = 0.5f;
		[Tooltip("The minimum head rotation required to apply a world redirection. Value is in °/s.")]
		public float RotationThreshold = 0f;
		[Tooltip("The speed threshold defining if the user is considered to be standing still or walking. Value is in m/s. [Hogson and Bachmann, 2013]")]
		public float WalkingThreshold = 0.2f;

		[Tooltip("The distance within which the dampening effect is applied on Redirected Walking (Hybrid). Value is in m. [Hogson and Bachmann, 2013]")]
		public float DampeningDistanceThreshold = 1.25f;
		[Tooltip("The dampening range. Value is in °.")]
		public float DampeningRange = 1.57f;
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
		public float CurvatureRadius = 15f;



		[Header("3D Interpolation")]
		[Tooltip("")]
		public Vector3 a;



		[Header("Pseudo-Haptic")]
		[Tooltip("The size of the area around the origin where the Swamp illusion defined by [Lécuyer et al., 2000] is applied. Value is in m.")]
		public float SwampSquareLength = 0.25f;
		[Tooltip("The C/D ratio inside the swamp area. Value is in m. [Lécuyer et al., 2000]")]
		public float SwampCDRatio = 0.75f;
	}
}