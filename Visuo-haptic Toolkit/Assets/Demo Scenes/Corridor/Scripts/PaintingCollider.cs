using UnityEngine;

namespace VHToolkit.Demo {
	public class PaintingCollider : MonoBehaviour {
		[SerializeField] private CorridorRedirection manager;
		[SerializeField] private CorridorRedirection.CorridorStates state;

		private void OnTriggerEnter(Collider other) {
			manager.SetState(state);
			this.GetComponent<Collider>().enabled = false;
		}
	}
}