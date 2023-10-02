using System.Collections.Generic;

using UnityEngine;
using BG.Redirection;

namespace BG.Visualisation {
	public class BRVisuals : MonoBehaviour {

		private static List<Color> colors;	// colors of the lines between the physical and virtual elements

		/// <summary>
		/// OnEnable is called once at the start of the game similarily to Start().
		/// Calling OnEnable instead of Start to support recompilation during play (Hot reload)
		/// </summary>
		private void OnEnable() {
			colors = new List<Color> () {
				Color.white,	// Indicates the redirection is within the detection thresholds
				Color.yellow	// Indicates the redirection is beyond the detection thresholds
			};
		}

		private void Update() {
			BodyRedirection rootScript = (BodyRedirection) Toolkit.Instance.rootScript;
			// draws threshold lines for the targets
			drawThresholdLines(rootScript.scene.physicalTarget.position, rootScript.scene.virtualTarget.position, rootScript);
			if (rootScript.IsRedirecting()) {
				// draws threshold lines for the hands
				drawThresholdLines(rootScript.scene.physicalHand.position, rootScript.scene.virtualHand.position, rootScript);
			}
		}

		/// <summary>
		/// Draws a white line between obj1 and obj2 only visible in editor. If the objects are too far apart according to
		/// Zenner et al. 2019, the lines turns yellow.
		/// </summary>
		/// <param name="obj1">Vector3: A physical GameObject's position</param>
		/// <param name="obj2">Vector3: The virtual corresponding GameObject's position</param>
		/// <param name="rootScript"></param>
		private void drawThresholdLines(Vector3 obj1, Vector3 obj2, BodyRedirection rootScript) {

            if (Debug.isDebugBuild) {			
				// Computes the euler angles from the rotation matrix from obj1 to obj2 around the origin
				Vector3 d = Quaternion.FromToRotation(rootScript.scene.origin.position - obj1, rootScript.scene.origin.position - obj2).eulerAngles;
				// Compares the euler angles against the thresholds and applies the correct color
				var allAnglesBelowThreshold = Mathf.Min(d.x, 360 - d.x) < Toolkit.Instance.parameters.MaxAngles.x &&
					 Mathf.Min(d.y, 360 - d.y) < Toolkit.Instance.parameters.MaxAngles.y &&
					 Mathf.Min(d.z, 360 - d.z) < Toolkit.Instance.parameters.MaxAngles.z;
				Debug.DrawLine(obj1, obj2, colors[allAnglesBelowThreshold ? 0 : 1]);
			}
		}
	}
}
