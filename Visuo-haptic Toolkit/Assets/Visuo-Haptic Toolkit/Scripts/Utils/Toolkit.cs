using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG.Redirection {
	/// <summary>
	/// Body redirection techniques.
	/// </summary>
	public enum BRTechnique {
		None,
		Han2018Instant,
		Han2018Continous,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom
	}
    /// <summary>
    /// World redirection techniques.
    /// </summary>
    public enum WRTechnique {
		None,
		Razzaque2001OverTimeRotation,
		Steinicke2008Translational,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World
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

		[HideInInspector] public Interaction rootScript;

		private void OnEnable() {
			if (Instance != null && Instance != this) {
				Destroy(this);
			}
			else {
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}

        private void Start() => rootScript = this.gameObject.GetComponent<Interaction>();

        public float CurvatureRadiusToRotationRate() => 360f / (2 * Mathf.PI * parameters.CurvatureRadius);

        public float CurvatureRadiusToRotationRate(float radius) => 360f / (2 * Mathf.PI * radius);
    }
}