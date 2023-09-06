using UnityEngine;

namespace BG.Redirection {
	[CreateAssetMenu(fileName = "Data", menuName = "VR Toolkit/Interaction Techniques Parameters", order = 1)]
	public class ParametersToolkit : ScriptableObject {

		[Header("Body Warping Thresholds")]
		[Tooltip("The detection thresholds measured by Zenner et al., 2019. The coordinate system is that of Unity, i.e. X: Letf/Right, Y: Up/Down, Z: Forward/Backward. Values are in degrees.")]
		public Vector3 MaxAngles;
		[Tooltip("A buffer value where the hand redirection is not applied [Han et al., 2018].")]
		public float NoRedirectionBuffer = 0.1f;

		[Header("3D Interpolation Thresholds")]
		// [Tooltip("")]
		// public Vector3 MaxAngles;

		[Header("World Warping Thresholds")]
		[Tooltip("The error in rotation where users are considered to be in the correct direction. Value is in °.")]
		public float RotationalEpsilon;
		[Tooltip("The maximum rotation that can be applied to the user's point of view in rotation along the vertical axis (Y). Value is in °/s.")]
		public float OverTimeRotaton;
		[Tooltip("The maximum gain in translation that can be applied to the user's point of view in translation (akin to a C/D ratio). Value has no unit and is not a percentage.")]
		public float GainsTranslational;
		[Tooltip("The maximum gain in rotation that can be applied to the user's point of view in rotation. Value has no unit and is not a percentage.")]
		public float GainsRotational;
		[Tooltip("The maximum gain in translation that can be applied to the user's point of view in rotation. Value is in °/m and is not a percentage.")]
		public float GainsCurvature;
	}
}