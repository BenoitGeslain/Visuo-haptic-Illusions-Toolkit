using UnityEngine;

namespace BG.Redirection {

	public enum WRTechnique {
		None,
		Razzaque2001OverTimeRotation,
		Steinicke2008Translational,
		Razzaque2001Rotational,
		Razzaque2001Curvature,
		Razzaque2001Hybrid,
	}

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
				default:
					Debug.LogError("Error Unknown Redirection technique.");
					break;
			}
		}

		private void Start() {
			init();
		}

		private void Update() {
			if (!reset && techniqueClass != null) {
				techniqueClass.Redirect(physicalTarget, virtualTarget, physicalHead, virtualHead);
			} else {
				// Reset virtualHand to physicalHand progressively
			}
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