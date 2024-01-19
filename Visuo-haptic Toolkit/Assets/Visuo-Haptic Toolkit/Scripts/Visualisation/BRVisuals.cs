using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VHToolkit.Redirection;

namespace VHToolkit.Visualisation {
	public class BRVisuals : MonoBehaviour {

		[SerializeField] private bool showThersholdLines;

		private BodyRedirection BRMainScript;
		private Scene scene;

		/// <summary>
		/// OnEnable is called once at the start of the game similarily to Start().
		/// Calling OnEnable instead of Start to support recompilation during play (Hot reload)
		/// </summary>
		private void OnEnable() {
			BRMainScript = GetComponent<BodyRedirection>();
			scene = BRMainScript.scene;
		}

		private void Update() {
			// draws threshold lines for the targets
			drawThresholdLines(BRMainScript.scene.physicalTarget.position, BRMainScript.scene.virtualTarget.position, BRMainScript);
			if (BRMainScript.IsRedirecting()) {
				// draws threshold lines for the hands
				foreach(Limb limb in BRMainScript.scene.limbs) {
					foreach(var vlimb in limb.virtualLimb)
						drawThresholdLines(limb.physicalLimb.position, vlimb.position, BRMainScript);
				}
			}
			if (BRMainScript.GetTechnique() == BRTechnique.Lecuyer2000Swamp) {
				foreach(var vlimb in BRMainScript.scene.limbs.SelectMany(limb => limb.virtualLimb)) {
					Vector3 distanceToOrigin = vlimb.position - BRMainScript.scene.origin.position;
					Color c = (MathF.Max(MathF.Abs(distanceToOrigin[0]), MathF.Abs(distanceToOrigin[2])) < Toolkit.Instance.parameters.SwampSquareLength/2) ?
							Color.green : Color.yellow;
					Debug.DrawRay(BRMainScript.scene.origin.position + new Vector3(Toolkit.Instance.parameters.SwampSquareLength/2, 0f, Toolkit.Instance.parameters.SwampSquareLength/2),
								Vector3.back * Toolkit.Instance.parameters.SwampSquareLength, c);
					Debug.DrawRay(BRMainScript.scene.origin.position + new Vector3(Toolkit.Instance.parameters.SwampSquareLength/2, 0f, Toolkit.Instance.parameters.SwampSquareLength/2),
								Vector3.left * Toolkit.Instance.parameters.SwampSquareLength, c);
					Debug.DrawRay(BRMainScript.scene.origin.position - new Vector3(Toolkit.Instance.parameters.SwampSquareLength/2, 0f, Toolkit.Instance.parameters.SwampSquareLength/2),
								Vector3.forward * Toolkit.Instance.parameters.SwampSquareLength, c);
					Debug.DrawRay(BRMainScript.scene.origin.position - new Vector3(Toolkit.Instance.parameters.SwampSquareLength/2, 0f, Toolkit.Instance.parameters.SwampSquareLength/2),
								Vector3.right * Toolkit.Instance.parameters.SwampSquareLength, c);
				}
			}
		}

		private void DrawLimbsRedirection() {
		//	scene.limbs.ForEach(limb => Debug.DrawLine());
		}

		/// <summary>
		/// Draws a white line between obj1 and obj2pol only visible in editor. If the objects are too far apart according to
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
				var colors = new Color[2] {Color.yellow, Color.white};
				Debug.DrawLine(obj1, obj2, colors[allAnglesBelowThreshold ? 0 : 1]);
			}
		}
	}
}
