using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// To reset redirection, set the technique enum to None.
	/// </summary>
	public class WorldRedirection : MonoBehaviour {

		[SerializeField] private WRTechnique technique;
		[SerializeField] private WorldRedirectionTechnique techniqueInstance;

		[Header("User Parameters")]
		[SerializeField] private Transform physicalHead;
		[SerializeField] private Transform virtualHead;
		private Quaternion previousOrientation;
		private Vector3 previousPosition;

		[Header("Technique Parameters")]
		[SerializeField] private WRStrategy strategy;
		[SerializeField] private WorldRedirectionStrategy strategyInstance;
		public Transform physicalTarget;
		public Transform virtualTarget;

		private void init() {
			switch (technique) {
				case WRTechnique.None:
					techniqueInstance = new ResetWorldRedirection();
					break;
				case WRTechnique.Razzaque2001OverTimeRotation:
					techniqueInstance = new Razzaque2001OverTimeRotation();
					break;
				case WRTechnique.Steinicke2008Translational:
					techniqueInstance = new Steinicke2008Translational();
					break;
				case WRTechnique.Razzaque2001Rotational:
					techniqueInstance = new Razzaque2001Rotational();
					break;
				case WRTechnique.Razzaque2001Curvature:
					techniqueInstance = new Razzaque2001Curvature();
					break;
				case WRTechnique.Razzaque2001Hybrid:
					techniqueInstance = new Razzaque2001Hybrid();
					break;
				case WRTechnique.Azmandian2016World:
					techniqueInstance = new Azmandian2016World();
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					break;
			}

			strategyInstance = strategy switch {
				WRStrategy.SteerToCenter => new SteerToCenter(),
				WRStrategy.SteerToOrbit => new SteerToOrbit(),
				WRStrategy.SteerToMultipleTargets => new SteerToMultipleTargets(),
				_ => null
			};

			previousOrientation = virtualHead.rotation;
			previousPosition = virtualHead.position;
		}

		private void Start() {
			init();
		}

		private void Update() {
			if (techniqueInstance != null) {
				techniqueInstance.Redirect(Vector3.forward, physicalHead, virtualHead, previousPosition, previousOrientation);
			}
			previousOrientation = virtualHead.rotation;
			previousPosition = virtualHead.position;
		}

		public void setTechnique(WRTechnique t) {
			technique = t;
			init();
		}

		public WRTechnique getTechnique(WRTechnique t) => technique;

		public bool IsRedirecting() {
			return Vector3.Distance(physicalHead.position, virtualHead.position) < Vector3.kEpsilon &&
				   Quaternion.Angle(physicalHead.rotation, virtualHead.rotation) < Vector3.kEpsilon;
		}
	}
}