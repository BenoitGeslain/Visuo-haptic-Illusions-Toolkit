using UnityEngine;

namespace BG.Redirection {
	[System.Serializable]
	public class XDirection {
		[SerializeField] private float right;
		[SerializeField] private float left;
	}

	[System.Serializable]
	public class YDirection {
		[SerializeField] private float up;
		[SerializeField] private float down;
	}

	[System.Serializable]
	public class ZDirection {
		[SerializeField] private float forward;
		[SerializeField] private float backward;
	}
}