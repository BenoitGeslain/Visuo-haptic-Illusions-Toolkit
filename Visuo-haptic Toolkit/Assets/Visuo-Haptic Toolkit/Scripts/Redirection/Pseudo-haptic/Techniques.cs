using System;

using UnityEngine;

// TODO once implemented move these techniques into the right namespace
// TODO handle collision detection
// TODO handle offset reduction
namespace VHToolkit.Redirection {

    /// <summary>
    /// This class implements the Swamp Illusion by Lécuyer et al., 2000. It is not a visuo-haptic illusion but a pseudo-haptic interaction techique.
    /// </summary>
    public class Lecuyer2000Swamp : BodyRedirectionTechnique {
        public override void Redirect(Scene scene) {
			// TODO use specific parameters, other gains
			Vector3 distanceToOrigin = scene.virtualHand.position - scene.origin.position;
			Vector3 instantTranslation = scene.GetHandInstantTranslation();
			if (MathF.Max(MathF.Abs(distanceToOrigin[0]), MathF.Abs(distanceToOrigin[2])) < Toolkit.Instance.parameters.SwampSquareLength/2) {
				scene.virtualHand.position += instantTranslation * Toolkit.Instance.parameters.SwampCDRatio;
			} else {
				scene.virtualHand.position += instantTranslation;
			}
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
            var v = scene.GetHandInstantTranslation();
            v[0] *= horizontalGain; v[1] *= verticalGain; v[2] *= horizontalGain;
            scene.virtualHand.Translate(v);
        }
    }
}