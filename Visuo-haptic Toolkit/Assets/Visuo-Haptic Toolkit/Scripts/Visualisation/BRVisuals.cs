using System.Collections.Generic;

using UnityEngine;

using BG.Redirection;

namespace BG.Visualisation {
	public class BRVisuals : MonoBehaviour {

		private static List<Color> colors;

		private void Start() {
			colors = new List<Color> () {
				Color.white,
				Color.yellow
			};
		}

		private void Update() {
			BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;
			drawThresholdLines(rootScript.physicalTarget.position, rootScript.virtualTarget.position, rootScript);
			if (rootScript.IsRedirecting()) {
				// drawThresholdLines(rootScript.physicalHand.position, rootScript.virtualHand.position, rootScript);
			}
		}

		private void drawThresholdLines(Vector3 obj1, Vector3 obj2, BodyRedirection rootScript) {
			Vector3 d = Quaternion.FromToRotation(rootScript.origin.position - obj1,
												  rootScript.origin.position - obj2).eulerAngles;
			Color c;
			if (Mathf.Min(d.x, 360-d.x) < Toolkit.Instance.parameters.MaxAngles.x &&
				Mathf.Min(d.y, 360-d.y) < Toolkit.Instance.parameters.MaxAngles.y &&
				Mathf.Min(d.z, 360-d.z) < Toolkit.Instance.parameters.MaxAngles.z) {
				c = colors[0];
			} else {
				c = colors[1];
			}

			Debug.DrawLine(obj1, obj2, c , 0.0f, true);
		}
	}
}
