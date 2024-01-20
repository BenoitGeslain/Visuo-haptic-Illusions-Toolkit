using UnityEngine;

namespace VHToolkit.Redirection {
	/// <summary>
	/// Available body redirection techniques.
	/// </summary>
	public enum BRTechnique {
		None,	// Hand Redirection techniques
		Reset,	// Hand Redirection techniques
		Han2018TranslationalShift,
		Han2018InterpolatedReach,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom,
		Poupyrev1996GoGo,
		_,	// Pseudo-haptic techiques
		Lecuyer2000Swamp,
		Samad2019Weight
	}

    /// <summary>
    /// Available world redirection techniques.
    /// </summary>
    public enum WRTechnique {
		None,
		Reset,
		Razzaque2001OverTimeRotation,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World,
		Steinicke2008Translational
	}

	public enum WRStrategy {
		NoSteering,
		SteerToCenter,
		SteerToOrbit,
		SteerToMultipleTargets,
		SteerInDirection
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
				DontDestroyOnLoad(gameObject);
			}
		}

        public float CurvatureRadiusToRotationRate() => CurvatureRadiusToRotationRate(parameters.CurvatureRadius);

        public static float CurvatureRadiusToRotationRate(float radius) => 180f / (Mathf.PI * radius);
    }
}