namespace VHToolkit.Redirection.Interpolation3D {

	public abstract class InterpolationTechnique : RedirectionTechnique { }

	public class Kohli2010RedirectedTouching : InterpolationTechnique {
		public override void Redirect(Scene scene) {
			var displace = ThinPlateSpline.SabooSmoothedDisplacementField(
				scene.reference,
				scene.interpolated,
				scene.parameters.smoothingParameter,
				scene.parameters.rescale
			);

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
