using UnityEngine;

namespace VHToolkit.Redirection {

	public class Interaction : MonoBehaviour {
		public Scene scene;
		[SerializeField] protected bool redirect = false;


        public void StartRedirection() => redirect = true;
    }
}