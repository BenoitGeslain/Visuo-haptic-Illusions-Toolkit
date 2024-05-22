using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VHToolkit.Redirection {

	[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public abstract class EditorRedirectionAttribute : System.Attribute { }
	public sealed class ShowHeadAttribute : EditorRedirectionAttribute { }
	public sealed class HasBufferAttribute : EditorRedirectionAttribute { }
	public sealed class HasThresholdAttribute : EditorRedirectionAttribute { }
	public sealed class HasStrategyAttribute : EditorRedirectionAttribute { }
	public sealed class HasTargetsAttribute : EditorRedirectionAttribute { }
	public enum BRTechnique {
		None,
		Reset,
		[InspectorName("")] SEPARATOR1, // Adds a visual separator in the drop-down inspector
										// Hand Redirection techniques
		[HasThreshold] Han2018TranslationalShift,
		[HasBuffer, HasThreshold] Han2018InterpolatedReach,
		[HasBuffer, HasThreshold] Azmandian2016Body,
		[ShowHead, HasThreshold] Azmandian2016Hybrid,
		[HasBuffer, HasThreshold] Cheng2017Sparse,
		[HasBuffer, HasThreshold] Geslain2022Polynom,
		[ShowHead] Poupyrev1996GoGo,
		[InspectorName(" ")] SEPARATOR2,
		[HasThreshold] Kohli2010RedirectedTouching,
		[InspectorName("  ")] SEPARATOR3,
		// Pseudo-haptic techiques
		Lecuyer2000Swamp,
		Samad2019Weight
	}

	/// <summary>
	/// Available world redirection techniques.
	/// </summary>
	public enum WRTechnique {
		None,
		Reset,
		[InspectorName("")] SEPARATOR1,
		[HasStrategy] Razzaque2001OverTimeRotation,
		[HasStrategy] Razzaque2001Rotational,
		[HasStrategy] Razzaque2001Curvature,
		[HasStrategy] Razzaque2001Hybrid,
		Azmandian2016World,
		Steinicke2008Translational
	}

	public enum WRStrategy {
		NoSteering,
		[InspectorName("")] SEPARATOR1,
		[HasTargets] SteerToCenter,
		SteerToOrbit,
		[HasTargets] SteerToMultipleTargets,
		SteerInDirection,
		[InspectorName(" ")] SEPARATOR2,
		APF_PushPull
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