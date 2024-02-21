using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.WorldRedirection;

namespace VHToolkit.Demo {
	public class CorridorRedirection : MonoBehaviour {

		private WorldRedirection redirectionScript;

		[SerializeField] private Transform UserHead;

		[SerializeField] private float redirectionApplied;
		[SerializeField] private List<Transform> paintingReferences;
		[Range(0, 360)]
		[SerializeField] private float redirectionAmount;
		[SerializeField] private Transform start, end;
		private float NormalizedDistance => Mathf.InverseLerp(start.position.z, end.position.z, UserHead.position.z);

		private void Start() {
			redirectionScript = Toolkit.Instance.gameObject.GetComponent<WorldRedirection>();
		}

		private void Update() {

			redirectionApplied = redirectionScript.GetAngularRedirection().eulerAngles.y;
			if (redirectionApplied > 180f)
				redirectionApplied = 360f - redirectionApplied;
			// If the correct redirection has been applied, stop the redirection
			if (Math.Abs(redirectionApplied) > redirectionAmount) {
				redirectionScript.StopRedirection();
			} else {
				redirectionScript.StartRedirection();
			}
			// Debug.Log(NormalizedDistance);
		}
	}
}