using UnityEngine;

namespace BG.Redirection {

	public class Interaction : MonoBehaviour {

		public virtual bool IsRedirecting() {
			Debug.LogError("Calling virtual function IsRedirecting() of Interaction class. This method must be overriden");
			return false;
		}
	}
}