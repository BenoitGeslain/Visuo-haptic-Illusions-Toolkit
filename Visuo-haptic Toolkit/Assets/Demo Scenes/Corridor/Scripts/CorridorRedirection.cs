using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VHToolkit;
using VHToolkit.Redirection;

public class CorridorRedirection : MonoBehaviour {
	private WorldRedirection redirectionScript;

	[SerializeField] private Transform UserHead;

	[SerializeField] private int paintingIndex = 0;
	[SerializeField] private List<Transform> paintingReferences;

	[Range(0, 45)]
	[SerializeField] private List<float> redirectionAmount;

	private void Start() {
		redirectionScript = Toolkit.Instance.gameObject.GetComponent<WorldRedirection>();
	}

	private void Update() {

		if (paintingIndex < 4 && UserHead.position.x > paintingReferences[paintingIndex].position.x) {
			paintingIndex++;
			redirectionScript.StartRedirection();
		}

		float redirectionApplied = redirectionScript.GetAngularRedirection().eulerAngles.y;
		if (redirectionApplied > 180f)
			redirectionApplied = 360f - redirectionApplied;

		Debug.Log(redirectionApplied);
		Debug.Log(redirectionApplied - redirectionAmount.Take(paintingIndex - 1).Sum());
		Debug.Log(redirectionAmount[paintingIndex - 1]);
		if (paintingIndex > 0 && Math.Abs(redirectionApplied) - redirectionAmount.Take(paintingIndex - 1).Sum() > redirectionAmount[paintingIndex - 1]) {
			redirectionScript.StopRedirection();
		}
	}
}