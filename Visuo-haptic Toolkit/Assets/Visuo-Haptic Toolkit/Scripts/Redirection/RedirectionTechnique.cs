namespace VHToolkit.Redirection {
	abstract public class RedirectionTechnique {
		/// <summary>
		/// <c>Redirect</c> redirects the Scene transforms according to the other parameters and the equations
		/// defined in the corresponding techniques. It needs to be overriden in children classes, and is called on Update() in *Redirection classes.
		/// </summary>
		public abstract void Redirect(Scene scene);
	}
}