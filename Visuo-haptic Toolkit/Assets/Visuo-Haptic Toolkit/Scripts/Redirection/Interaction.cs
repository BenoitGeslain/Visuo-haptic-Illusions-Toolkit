using System.Collections.Generic;

using UnityEngine;

namespace VHToolkit.Redirection {

	/// <summary>
	/// This is the base class for the visuo-haptic techniques managers such as BodyRedirection.
	/// </summary>
    public class Interaction : MonoBehaviour {
		public Scene scene;
		[SerializeField] protected bool redirect = false;

        public void StartRedirection() => redirect = true;

		public List<Transform> GetPhysicalLimbs() => scene.limbs.ConvertAll(limb => limb.physicalLimb);

		public void AddPhysicalLimb(Transform physicalLimb, List<Transform> virtualLimbs) => scene.limbs.Add(new(physicalLimb, virtualLimbs));

		public void RemovePhysicalLimb(Transform physicalLimb) => scene.limbs.RemoveAll(x => x.physicalLimb == physicalLimb);

		public List<Limb> GetLimbs() => scene.limbs;

		public void SetLimbs(List<Limb> limbs) => scene.limbs = limbs;
    }
}