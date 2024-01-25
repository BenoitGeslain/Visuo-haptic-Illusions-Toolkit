using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VHToolkit;
using VHToolkit.Redirection;

public class CorridorRedirection : MonoBehaviour {

	public enum CorridorStates {
		Calibration = -1,
		None,
		FirstPainting,
		SecondPainting,
		ThirdPainting,
		FourthPainting
	}

	private WorldRedirection redirectionScript;

	[SerializeField] private Transform UserHead;

	public CorridorStates state = CorridorStates.None;
	[ReadOnly][SerializeField] private float redirectionApplied;
	[SerializeField] private List<Transform> paintingReferences;
	[Range(0, 45)]
	[SerializeField] private List<float> redirectionAmount;
	private int nPaintings;

	private void Start() {
		redirectionScript = Toolkit.Instance.gameObject.GetComponent<WorldRedirection>();

		nPaintings = paintingReferences.Count;
		if (paintingReferences.Count != redirectionAmount.Count)
			Debug.LogWarning("Different numbers of painting references and redirection amounts.");
	}

	private void Update() {

		if (state != CorridorStates.Calibration) {
			int currentPainting = (int)state - 1;
			int nextPainting = (int)state;

			redirectionApplied = redirectionScript.GetAngularRedirection().eulerAngles.y;
			if (redirectionApplied > 180f)
				redirectionApplied = 360f - redirectionApplied;

			// Debug.Log((Math.Abs(redirectionApplied) - redirectionAmount.Take(currentPainting).Sum()) + ", " + redirectionAmount[currentPainting]);
			// If the correct redirection has been applied, stop the redirection
			if (currentPainting >= 0 && Math.Abs(redirectionApplied) - redirectionAmount.Take(currentPainting).Sum() > redirectionAmount[currentPainting]) {
				redirectionScript.StopRedirection();
			}
		}
	}

	public void SetState(CorridorStates s) {
		state = (CorridorStates)Math.Max((int)state, (int)s);
		redirectionScript.StartRedirection();
	}
}