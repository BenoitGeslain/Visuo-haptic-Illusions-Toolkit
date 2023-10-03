using System.Collections;
using UnityEngine;

namespace BG.Redirection {
    abstract public class RedirectionTechnique {
        /// <summary>
        /// This virtual function applies the redirection to the Scene transforms according to the other parameters and the equations
        /// defined in the corresponding techniques. It needs to be overriden by a child class and it is called on Update() in *Redirection classes.
        /// </summary>
        public virtual void Redirect(Scene scene) {
            Debug.LogError("Calling Redirect() virtual method. It should be overriden");
        }
    }
}