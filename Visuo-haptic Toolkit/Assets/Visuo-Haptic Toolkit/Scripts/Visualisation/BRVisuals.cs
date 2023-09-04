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
				drawThresholdLines(rootScript.physicalHand.position, rootScript.virtualHand.position, rootScript);
			}
		}

		private void drawThresholdLines(Vector3 obj1, Vector3 obj2, BodyRedirection rootScript) {
			Vector3 d = Quaternion.FromToRotation(rootScript.origin.position - obj1,
												  rootScript.origin.position - obj2).eulerAngles;
			Debug.Log(d);
			Debug.Log(Toolkit.Instance.parameters.MaxAngles);
			Color c;
			if (d.x < Toolkit.Instance.parameters.MaxAngles.x && d.y < Toolkit.Instance.parameters.MaxAngles.y && d.z < Toolkit.Instance.parameters.MaxAngles.z) {
				c = colors[0];
			} else {
				c = colors[1];
			}

			Debug.DrawLine(obj1,
						   obj2,
						   c , 0.0f, true);
		}
	}
}
