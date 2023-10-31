using UnityEngine;

namespace BG.Redirection {
	/// <summary>
	/// Available body redirection techniques.
	/// </summary>
	public enum BRTechnique {
		Reset,
		Han2018TranslationalShift,
		Han2018InterpolatedReach,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom,
		Poupyrev1996GoGo
	}
    /// <summary>
    /// Available world redirection techniques.
    /// </summary>
    public enum WRTechnique {
		Reset,
		Razzaque2001OverTimeRotation,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World,
		Steinicke2008Translational
	}

	public enum WRStrategy {
		None,
		SteerToCenter,
		SteerToOrbit,
		SteerToMultipleTargets
	}

	public class Toolkit : MonoBehaviour {
		public static Toolkit Instance { get; private set; }

		public ParametersToolkit parameters;

		private void OnEnable() {
			if (Instance != null && Instance != this) {
				Destroy(this);
			}
			else {
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}

        public float CurvatureRadiusToRotationRate() => CurvatureRadiusToRotationRate(parameters.CurvatureRadius);

        public static float CurvatureRadiusToRotationRate(float radius) => 360f / (2 * Mathf.PI * radius);
    }
}