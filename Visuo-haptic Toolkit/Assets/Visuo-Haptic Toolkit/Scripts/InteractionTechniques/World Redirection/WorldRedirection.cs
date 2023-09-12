using UnityEngine;

namespace BG.Redirection {

	/// <summary>
	/// This class allows users to select through the inspector or set through the API which
	/// body redirection technique to use as well as the relevant parameters.
	/// </summary>
	public class WorldRedirection : MonoBehaviour {

		[SerializeField] private WRTechnique technique;
		[SerializeField] private WorldRedirectionTechnique techniqueClass;

		[Header("User Parameters")]
		[SerializeField] private Transform physicalHead;
		[SerializeField] private Transform virtualHead;
		private float previousFrameYOrientation;
		private Vector3 previousPosition;

		[Header("Technique Parameters")]
		public Transform physicalTarget;
		public Transform virtualTarget;

		private bool reset;

		private void init() {
			switch (technique) {
				case WRTechnique.None:
					techniqueClass = null;
					break;
				case WRTechnique.Razzaque2001OverTimeRotation:
					techniqueClass = new Razzaque2001OverTimeRotation();
					break;
				case WRTechnique.Steinicke2008Translational:
					techniqueClass = new Steinicke2008Translational();
					break;
				case WRTechnique.Razzaque2001Rotational:
					techniqueClass = new Razzaque2001Rotational();
					break;
				case WRTechnique.Razzaque2001Curvature:
					techniqueClass = new Razzaque2001Curvature();
					break;
				case WRTechnique.Razzaque2001Hybrid:
					techniqueClass = new Razzaque2001Hybrid();
					break;
				case WRTechnique.Azmandian2016World:
					techniqueClass = new Azmandian2016World();
					break;
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					break;
			}

			previousFrameYOrientation = virtualHead.eulerAngles.y;
			previousPosition = virtualHead.position;
		}

		private void Start() {
			init();
		}

		private void Update() {
			if (!reset && techniqueClass != null) {
				techniqueClass.Redirect(Vector3.forward, previousPosition, previousFrameYOrientation, physicalHead, virtualHead);
			} else {
				// Reset virtualHand to physicalHand progressively
			}
			previousFrameYOrientation = virtualHead.eulerAngles.y;
		}

		public void setTechnique(WRTechnique t) {
			technique = t;
			init();
		}
		public WRTechnique getTechnique(WRTechnique t) {
			return technique;
		}

		public bool IsRedirecting() {
			return Vector3.Distance(physicalHead.position, virtualHead.position) < Vector3.kEpsilon;
				   // && test orientation;
		}

		public void resetRedirection() {
			reset = true;
		}

		public void restartRedirection() {
			if (reset) {
				Debug.LogWarning("Redirection should be reset before restarted.");
				return;
			} else if (IsRedirecting()) {
				Debug.LogWarning("Redirection is not reset yet");
				return;
			}
			reset = false;
		}
	}
}