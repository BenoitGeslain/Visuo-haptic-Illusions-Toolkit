using System;

using UnityEngine;

using VHToolkit.Redirection;
using VHToolkit.Redirection.BodyRedirection;

namespace VHToolkit.Visualisation {
	public class BRVisuals : MonoBehaviour {

		private BodyRedirection BRMainScript;
		private Scene scene;

		/// <summary>
		/// OnEnable is called once at the start of the game similarily to Start().
		/// Calling OnEnable instead of Start to support recompilation during play (Hot reload)
		/// </summary>
		private void Start() {
			this.enabled &= Debug.isDebugBuild;
			BRMainScript = GetComponent<BodyRedirection>();
			scene = BRMainScript.scene;
		}

		private void Update() {
			// draws threshold lines for the targets
			DrawThresholdLines(scene.physicalTarget.position, scene.virtualTarget.position);


			if (BRMainScript.IsRedirecting()) {
				// draws threshold lines for the hands
				foreach(Limb limb in scene.limbs) {
					DrawThresholdLines(limb.physicalLimb.position, limb.virtualLimb[0].position);
				}
			}

			// draw Lécuyer's swamp if applicable
			if (BRMainScript.Technique == BRTechnique.Lecuyer2000Swamp) {
				foreach(var vlimb in scene.virtualLimbs) {
					Vector3 distanceToOrigin = vlimb.position - scene.origin.position;
					Color c = (MathF.Max(MathF.Abs(distanceToOrigin[0]), MathF.Abs(distanceToOrigin[2])) < scene.parameters.SwampSquareLength/2) ?
							Color.green : Color.yellow;
					Debug.DrawRay(scene.origin.position + new Vector3(scene.parameters.SwampSquareLength/2, 0f, scene.parameters.SwampSquareLength/2),
								Vector3.back * scene.parameters.SwampSquareLength, c);
					Debug.DrawRay(scene.origin.position + new Vector3(scene.parameters.SwampSquareLength/2, 0f, scene.parameters.SwampSquareLength/2),
								Vector3.left * scene.parameters.SwampSquareLength, c);
					Debug.DrawRay(scene.origin.position - new Vector3(scene.parameters.SwampSquareLength/2, 0f, scene.parameters.SwampSquareLength/2),
								Vector3.forward * scene.parameters.SwampSquareLength, c);
					Debug.DrawRay(scene.origin.position - new Vector3(scene.parameters.SwampSquareLength/2, 0f, scene.parameters.SwampSquareLength/2),
								Vector3.right * scene.parameters.SwampSquareLength, c);
				}
			}
		}

		/// <summary>
		/// Draws a white line between obj1 and obj2pol only visible in editor. If the objects are too far apart according to
		/// Zenner et al. 2019, the lines turns yellow.
		/// </summary>
		/// <param name="obj1">Vector3: A physical GameObject's position</param>
		/// <param name="obj2">Vector3: The virtual corresponding GameObject's position</param>
		private void DrawThresholdLines(Vector3 obj1, Vector3 obj2) {
			// Computes the euler angles from the rotation matrix from obj1 to obj2 around the origin
			Vector3 d = Quaternion.FromToRotation(scene.origin.position - obj1, scene.origin.position - obj2).eulerAngles;

			// Compares the euler angles against the thresholds and applies the correct color
			//TODO faux tel quel, il faut regarder si c'est à gauche ou à droite par rapport à l'axe origine-target
			var allAnglesBelowThreshold = 360 - d.x < scene.parameters.HorizontalAngles.left &&
										  d.x < scene.parameters.HorizontalAngles.right &&
										  360 - d.y < scene.parameters.VerticalAngles.up &&
										  d.y < scene.parameters.VerticalAngles.down &&
										  360 - d.z < scene.parameters.DepthGain.forward &&
										  d.z < scene.parameters.DepthGain.backward;

			Debug.DrawLine(obj1, obj2, allAnglesBelowThreshold ? Color.yellow : Color.white);
		}
	}
}
