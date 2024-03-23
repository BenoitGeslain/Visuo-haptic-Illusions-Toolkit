using UnityEngine;

namespace VHToolkit.Redirection.Interpolation3D {
	/// <summary>
	/// This is the abstract base class for 3D interpolation techniques.
	/// </summary>
	public abstract class InterpolationTechnique : RedirectionTechnique { }

	/// <summary>
	/// This class implements the "Redirected Touching" effect (Kohli 2010), in which space is locally remapped so as to agree with the deformation of an input mesh.
	/// </summary>
	public class Kohli2010RedirectedTouching : InterpolationTechnique {

		System.Func<Vector3, Vector3> displace = null;

		/// <summary>
		/// Compute the displacement vector field, which should be done only on init and whenever its parameters change for efficiency reasons.
		/// </summary>
		public void ComputeDisplacement(Scene scene) {
			displace = ThinPlateSpline.SabooSmoothedDisplacementField(
				scene.reference,
				scene.interpolated,
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

	public class NoInterpolation : InterpolationTechnique {
		public override void Redirect(Scene scene) {
			scene.limbs.ForEach(limb => limb.virtualLimb.ForEach(vLimb => vLimb.position = limb.physicalLimb.position));
		}
	}
}
