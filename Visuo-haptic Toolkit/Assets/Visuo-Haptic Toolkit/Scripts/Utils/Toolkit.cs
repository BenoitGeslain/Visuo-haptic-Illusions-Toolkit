using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG.Redirection {
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