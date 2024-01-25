using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VHToolkit.Redirection {

	/// <summary>
	/// This is the base class for the visuo-haptic techniques managers such as BodyRedirection.
	/// </summary>
    public class Interaction : MonoBehaviour {
		public Scene scene;
		public bool redirect = false;

		public void StartRedirection() => redirect = true;

		public void StopRedirection() => redirect = false;

		public List<Transform> GetPhysicalLimbs() => scene.limbs.ConvertAll(limb => limb.physicalLimb);

		public void AddPhysicalLimb(Transform physicalLimb, IEnumerable<Transform> virtualLimbs) => scene.limbs.Add(new(physicalLimb, virtualLimbs.ToList()));

		public void RemovePhysicalLimb(Transform physicalLimb) => scene.limbs.RemoveAll(x => x.physicalLimb == physicalLimb);

		public List<Limb> GetLimbs() => scene.limbs;

		public void SetLimbs(IEnumerable<Limb> limbs) => scene.limbs = limbs.ToList();
    }
}