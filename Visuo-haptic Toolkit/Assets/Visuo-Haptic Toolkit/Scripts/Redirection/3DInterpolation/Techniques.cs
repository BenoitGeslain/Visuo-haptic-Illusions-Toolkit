using System;
using UnityEngine;

using VHToolkit.Redirection.BodyRedirection;

namespace VHToolkit.Redirection.Interpolation3D {

	/// <summary>
	/// This class implements the "Redirected Touching" effect (Kohli 2010), in which space is locally remapped so as to agree with the deformation of an input mesh.
	/// </summary>
	public class Kohli2010RedirectedTouching : BodyRedirectionTechnique {

		System.Func<Vector3, Vector3> displace = null;

		/// <summary>
		/// Compute the displacement vector field, which should be done only on init and whenever its parameters change for efficiency reasons.
		/// </summary>
		public void ComputeDisplacement(Scene scene) {
			displace = ThinPlateSpline.SabooSmoothedDisplacementField(
				Array.ConvertAll(scene.referenceParent.GetComponentsInChildren<Transform>(), t => t.position),
				Array.ConvertAll(scene.interpolatedParent.GetComponentsInChildren<Transform>(), t => t.position),
				scene.parameters.smoothingParameter,
				scene.parameters.rescale
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
