using System;
using System.Linq;

using UnityEngine;


// TODO handle collision detection
// TODO handle offset reduction
namespace VHToolkit.Redirection {

    /// <summary>
    /// This class implements the Swamp Illusion by Lécuyer et al., 2000. It is not a visuo-haptic illusion but a pseudo-haptic interaction techique.
    /// </summary>
    public class Lecuyer2000Swamp : BodyRedirectionTechnique {
        public override void Redirect(Scene scene) {
            foreach ((var limb, var t) in scene.limbs.Zip(scene.GetHandInstantTranslation(), (limb, t) => (limb, t))) {
                foreach(Transform vlimb in limb.virtualLimb) {
                    var distanceToOrigin = vlimb.position - scene.origin.position;
                    bool insideSwamp = MathF.Max(MathF.Abs(distanceToOrigin[0]), MathF.Abs(distanceToOrigin[2])) * 2 < Toolkit.Instance.parameters.SwampSquareLength;
                    vlimb.Translate(insideSwamp ? t * Toolkit.Instance.parameters.SwampCDRatio : t, relativeTo: Space.World);
			    }
            }
        }
    }

    /// <summary>
    /// This class implements the pseudo-haptic weight effect by Samad et al., 2019. CD ratio manipulation simulates the effects of weight.
    /// </summary>
    public class Samad2019Weight : BodyRedirectionTechnique {
        public override void Redirect(Scene scene) {
            float ratio = 0.65f;
            float normalizedMass = 1.5f; // high is heavy, low is light (though what this means is unclear)
            var gainVector = new Vector3(ratio, 1f, ratio) / normalizedMass;
            foreach((var limb, var t) in scene.limbs.Zip(scene.GetHandInstantTranslation(), (limb, t) => (limb, t))) {
                limb.virtualLimb.ForEach(vLimb => vLimb.Translate(Vector3.Scale(t, gainVector), relativeTo: Space.World));
            }
        }
    }
}