using System.Collections.Generic;

using UnityEngine;
using BG.Redirection;

namespace BG.Visualisation {
	public class WRVisuals : MonoBehaviour {

		private static List<Color> colors;	// colors of the lines between the physical and virtual elements

		// Calling Onenable instead of Start to support recompilation during play
		private void OnEnable() {
			colors = new List<Color> () {
				Color.white,	// Indicates the redirection is within the detection thresholds
				Color.yellow	// Indicates the redirection is beyond the detection thresholds
			};
		}

		private void Update() {
			WorldRedirection rootScript = (WorldRedirection) Toolkit.Instance.rootScript;
			// draws threshold lines for the targets
			drawDirectionLines(rootScript);
		}

		private void drawDirectionLines(WorldRedirection rootScript) {
			Vector3 dir = rootScript.strategyInstance.targets[0].position;
			dir.y = rootScript.physicalHead.position.y;
			dir -= rootScript.physicalHead.position;

			Debug.DrawRay(rootScript.physicalHead.position, dir, colors[0]);
			Debug.DrawRay(rootScript.physicalHead.position, rootScript.physicalHead.forward * dir.magnitude, colors[1]);
		}
	}
}
