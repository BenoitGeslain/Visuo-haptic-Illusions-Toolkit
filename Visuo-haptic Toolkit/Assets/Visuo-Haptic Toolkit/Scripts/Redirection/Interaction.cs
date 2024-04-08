using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VHToolkit.Redirection {
	public enum BRTechnique {
		None,
		Reset,
		[InspectorName("")] SEPARATOR1, // Adds a visual separator in the drop-down inspector
		// Hand Redirection techniques
		Han2018TranslationalShift,
		Han2018InterpolatedReach,
		Azmandian2016Body,
		Azmandian2016Hybrid,
		Cheng2017Sparse,
		Geslain2022Polynom,
		Poupyrev1996GoGo,
		[InspectorName(" ")] SEPARATOR2,
		// Pseudo-haptic techiques
		Lecuyer2000Swamp,
		Samad2019Weight
	}
	/// <summary>
	/// Available 3D Interpolation redirection techniques.
	/// </summary>
	public enum ThreeDTechnique {
		None,
		Kohli2010RedirectedTouching
	}

	/// <summary>
	/// Available world redirection techniques.
	/// </summary>
	public enum WRTechnique {
		None,
		Reset,
		[InspectorName("")] SEPARATOR1,
		Razzaque2001OverTimeRotation,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
		Azmandian2016World,
		Steinicke2008Translational
	}

	public enum WRStrategy {
		NoSteering,
		[InspectorName("")] SEPARATOR1,
		SteerToCenter,
		SteerToOrbit,
		SteerToMultipleTargets,
		SteerInDirection,
		[InspectorName(" ")] SEPARATOR2,
		PushPullReactive
	}

	public enum HybridAggregate {
		Max,
		[InspectorName("")] SEPARATOR1,
		Sum,
		Mean,
		WeightedSum
	}

	/// <summary>
	/// This is the base class for the visuo-haptic techniques managers such as BodyRedirection.
	/// </summary>
	public class Interaction : MonoBehaviour {
		public Scene scene;
		public bool redirect = false;

		public List<Transform> GetPhysicalLimbs() => scene.limbs.ConvertAll(limb => limb.physicalLimb);

		public void AddPhysicalLimb(Transform physicalLimb, IEnumerable<Transform> virtualLimbs) => scene.limbs.Add(new(physicalLimb, virtualLimbs.ToList()));

		public void RemovePhysicalLimb(Transform physicalLimb) => scene.limbs.RemoveAll(x => x.physicalLimb == physicalLimb);

		public List<Limb> GetLimbs() => scene.limbs;

		public void SetLimbs(IEnumerable<Limb> limbs) => scene.limbs = limbs.ToList();
	}
}