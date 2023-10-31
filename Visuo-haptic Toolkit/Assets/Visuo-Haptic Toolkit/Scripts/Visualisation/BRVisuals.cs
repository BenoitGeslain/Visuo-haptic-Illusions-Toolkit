using System.Collections.Generic;

using UnityEngine;
using VHToolkit.Redirection;

namespace VHToolkit.Visualisation {
	public class BRVisuals : MonoBehaviour {

		[SerializeField] private bool showThersholdLines;

		private BodyRedirection BRMainScript;

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

			BRMainScript = GetComponent<BodyRedirection>();
		}

		private void Update() {
			// draws threshold lines for the targets
			drawThresholdLines(BRMainScript.scene.physicalTarget.position, BRMainScript.scene.virtualTarget.position, BRMainScript);
			if (BRMainScript.IsRedirecting()) {
				// draws threshold lines for the hands
				drawThresholdLines(BRMainScript.scene.physicalHand.position, BRMainScript.scene.virtualHand.position, BRMainScript);
			}
			if (BRMainScript.technique == BRTechnique.Lecuyer2000Swamp) {
				Debug.DrawRay(BRMainScript.scene.origin.position + new Vector3(Toolkit.Instance.parameters.SwampSquareDistance/2, 0f, Toolkit.Instance.parameters.SwampSquareDistance/2),
							  Vector3.back * Toolkit.Instance.parameters.SwampSquareDistance, colors[1]);
				Debug.DrawRay(BRMainScript.scene.origin.position + new Vector3(Toolkit.Instance.parameters.SwampSquareDistance/2, 0f, Toolkit.Instance.parameters.SwampSquareDistance/2),
							  Vector3.left * Toolkit.Instance.parameters.SwampSquareDistance, colors[1]);
				Debug.DrawRay(BRMainScript.scene.origin.position - new Vector3(Toolkit.Instance.parameters.SwampSquareDistance/2, 0f, Toolkit.Instance.parameters.SwampSquareDistance/2),
							  Vector3.forward * Toolkit.Instance.parameters.SwampSquareDistance, colors[1]);
				Debug.DrawRay(BRMainScript.scene.origin.position - new Vector3(Toolkit.Instance.parameters.SwampSquareDistance/2, 0f, Toolkit.Instance.parameters.SwampSquareDistance/2),
							  Vector3.right * Toolkit.Instance.parameters.SwampSquareDistance, colors[1]);
			}
		}

		/// <summary>
		/// Draws a white line between obj1 and obj2 only visible in editor. If the objects are too far apart according to
		/// Zenner et al. 2019, the lines turns yellow.
		/// </summary>
		/// <param name="obj1">Vector3: A physical GameObject's position</param>
		/// <param name="obj2">Vector3: The virtual corresponding GameObject's position</param>
		/// <param name="BRMainScript"></param>
		private void drawThresholdLines(Vector3 obj1, Vector3 obj2, BodyRedirection BRMainScript) {

            if (Debug.isDebugBuild) {
				// Computes the euler angles from the rotation matrix from obj1 to obj2 around the origin
				Vector3 d = Quaternion.FromToRotation(BRMainScript.scene.origin.position - obj1, BRMainScript.scene.origin.position - obj2).eulerAngles;
				// Compares the euler angles against the thresholds and applies the correct color
				//TODO faux tel quel, il faut regarder si c'est à gauche ou à droite par rapport à l'axe origine-target
				var allAnglesBelowThreshold = 360 - d.x < Toolkit.Instance.parameters.HorizontalAngles.left &&
											  d.x < Toolkit.Instance.parameters.HorizontalAngles.right &&
											  360 - d.y < Toolkit.Instance.parameters.VerticalAngles.up &&
											  d.y < Toolkit.Instance.parameters.VerticalAngles.down &&
											  360 - d.z < Toolkit.Instance.parameters.Gain.faster &&
											  d.z < Toolkit.Instance.parameters.Gain.slower;
				Debug.DrawLine(obj1, obj2, colors[allAnglesBelowThreshold ? 0 : 1]);
			}
		}
	}
}
