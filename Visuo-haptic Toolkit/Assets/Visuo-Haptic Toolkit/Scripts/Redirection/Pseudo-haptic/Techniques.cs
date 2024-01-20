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
            scene.limbs.Zip(scene.GetHandInstantTranslation(), (limb, t) => (limb, t)).ToList().ForEach(p => {
                foreach(Transform vlimb in p.limb.virtualLimb) {
                    var distanceToOrigin = vlimb.position - scene.origin.position;
                    bool insideSwamp = MathF.Max(MathF.Abs(distanceToOrigin[0]), MathF.Abs(distanceToOrigin[2])) * 2 < Toolkit.Instance.parameters.SwampSquareLength;
                    vlimb.Translate(insideSwamp ? p.t * Toolkit.Instance.parameters.SwampCDRatio : p.t);
			    }
            });
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public class Samad2019Weight : BodyRedirectionTechnique {
        public override void Redirect(Scene scene) {
            float ratio = 0.65f;
            float normalizedMass = 1.5f; // high is heavy, low is light (though what this means is unclear)
            float verticalGain = 1 / normalizedMass;
            float horizontalGain = verticalGain * ratio;
            Vector3 gainVector = new(horizontalGain, verticalGain, horizontalGain);
            foreach(var p in scene.limbs.Zip(scene.GetHandInstantTranslation(), (limb, t) => (limb, t))) {
                p.limb.virtualLimb.ForEach(vLimb => vLimb.Translate(Vector3.Scale(p.t, gainVector)));
            }
        }
    }
}