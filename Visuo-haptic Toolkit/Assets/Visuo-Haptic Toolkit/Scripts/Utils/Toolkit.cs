using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG.Redirection {

	public enum BRTechnique {
		None,
		Han2018Instant,
		Han2018Continous,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom
	}

	public enum WRTechnique {
		None,
		Razzaque2001OverTimeRotation,
		Steinicke2008Translational,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World,
	}

	public class Toolkit : MonoBehaviour {
		public static Toolkit Instance { get; private set; }

		public ParametersToolkit parameters;

		[HideInInspector] public Interaction rootScript;

		private void Awake() {
			if (Instance != null && Instance != this) {
				Destroy(this);
			}
			else {
				Instance = this;
			}
		}

        private void Start() => rootScript = this.gameObject.GetComponent<Interaction>();
    }
}