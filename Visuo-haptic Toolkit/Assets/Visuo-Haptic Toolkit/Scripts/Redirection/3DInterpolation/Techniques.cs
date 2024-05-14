using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using VHToolkit.Redirection.BodyRedirection;

namespace VHToolkit.Redirection.Interpolation3D {

	/// <summary>
	/// This class implements the "Redirected Touching" effect (Kohli 2010), in which space is locally remapped so as to agree with the deformation of an input mesh.
	/// </summary>
	public class Kohli2010RedirectedTouching : BodyRedirectionTechnique {

		Func<Vector3, Vector3> displace = null;
		bool add_boundaries = true;

		/// <summary>
		/// Compute the displacement vector field, which should be done only on init and whenever its parameters change for efficiency reasons.
		/// </summary>
		public void ComputeDisplacement(Scene scene) {
			var x = Array.ConvertAll(scene.referenceParent.GetComponentsInChildren<Transform>(), t => t.position);
			var y = Array.ConvertAll(scene.interpolatedParent.GetComponentsInChildren<Transform>(), t => t.position);
			displace = ThinPlateSpline.SabooSmoothedDisplacementField(
				x,
				y,
				scene.parameters.SmoothingParameter,
				scene.parameters.Rescale
			);
			if (add_boundaries) {
				var bounds = new Bounds(x[0], Vector3.zero);
				foreach (var v in x.Concat(y)) {
					bounds.Encapsulate(v);
				}
				var sqrRadius = 3 * bounds.extents.sqrMagnitude;

				static float f(float x) => x > 0 ? Mathf.Exp(-1 / x) : 0;
				static float g(float x) => f(x) / (f(x) + f(1 - x));
				float transitionLayerWidth = 0.5F;
				float smoothTransition(float x) => 1 - g((x - sqrRadius) / (transitionLayerWidth * sqrRadius));
				var old_displace = displace;
				displace = (pos) => pos + (old_displace(pos) - pos) * smoothTransition((pos - bounds.center).sqrMagnitude);
			}
		}


		public override void Redirect(Scene scene) {
			if (displace is null) {
				ComputeDisplacement(scene);
			}

			foreach (var limb in scene.limbs) {
				var newPosition = displace(limb.physicalLimb.position);
				limb.virtualLimb.ForEach(vLimb => vLimb.position = newPosition);
			}
		}
	}
}
