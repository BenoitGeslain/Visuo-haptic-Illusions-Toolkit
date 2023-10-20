using BG.Redirection;
using UnityEditor;
using UnityEngine;

// TODO once implemented move these techniques into the right namespace
// TODO handle collision detection
// TODO handle offset reduction
namespace Assets.Visuo_Haptic_Toolkit.Scripts.Utils.WorkInProgress {
    public class Samad2019PseudoHapticWeight : BodyRedirectionTechnique {
        public void Redirect(Scene scene) {
            float ratio = 0.65f;
            float normalizedMass = 5f; // above 1 is heavy, below 1 is light (though what this means is unclear) 
            float verticalGain = 1 / normalizedMass;
            float horizontalGain = verticalGain * ratio;
            var v = scene.GetHandInstantTranslation();
            v[0] *= horizontalGain; v[1] *= verticalGain; v[2] *= horizontalGain;
            scene.virtualHand.Translate(v);
        }
    }
}