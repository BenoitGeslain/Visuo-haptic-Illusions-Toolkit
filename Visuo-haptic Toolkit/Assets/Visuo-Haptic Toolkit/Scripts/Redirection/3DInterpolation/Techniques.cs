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
			if (add_boundaries) {
				var bounds = new Bounds(x[0], Vector3.zero);
				foreach (var v in x.Concat(y)) {
					bounds.Encapsulate(v);
				}
				bounds.extents *= 2f;
				Vector3[] ends = { bounds.min, bounds.max };
				var supplementary_fixed = new List<Vector3>(8);
				foreach (var a in ends)
					foreach (var b in ends)
						foreach (var c in ends)
							supplementary_fixed.Add(new Vector3(a.x, b.y, c.z));
				x = x.Concat(supplementary_fixed).ToArray();
				y = y.Concat(supplementary_fixed).ToArray();
			}
			displace = ThinPlateSpline.SabooSmoothedDisplacementField(
				x,
				y,
				scene.parameters.SmoothingParameter,
				scene.parameters.Rescale
			);
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
